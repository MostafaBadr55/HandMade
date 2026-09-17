using HandMade.Application.Features.Orders.Commands.AcceptOrderQuote;
using HandMade.Application.Features.Orders.Queries.GetOrderStatus;
using HandMade.Application.Features.Payments.Commands.CreatePayment;
using HandMade.Application.Features.Payments.Commands.RefundPayment;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using MediatR;

namespace HandMade.Application.Features.Orders.Orchestrators.AcceptQuoteAndPayAction
{
    /// <summary>
    /// The client accepts the artist quote and pays in one move. Taking the money and
    /// moving the order must not come apart: if the transition fails after the charge
    /// succeeded, the charge is refunded — the money equivalent of the orphan-file
    /// cleanup in CreateProductOrchestrator.
    /// </summary>
    public record AcceptQuoteAndPayOrchestrator(
        Guid UserId,
        Guid OrderId,
        PaymentMethod Method) : IRequest<RequestResult<bool>>;

    public class AcceptQuoteAndPayOrchestratorHandler(IMediator _mediator, IUnitOfWork _unitOfWork)
        : IRequestHandler<AcceptQuoteAndPayOrchestrator, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            AcceptQuoteAndPayOrchestrator request,
            CancellationToken cancellationToken)
        {
            // Refuse an illegal transition BEFORE touching money. Without this the
            // card is charged and then refunded just to discover the order was not
            // awaiting the buyer — churn the client would see on their statement.
            var status = await _mediator.Send(
                new GetOrderStatusQuery(request.UserId, request.OrderId), cancellationToken);

            if (!status.IsSuccess)
                return RequestResult<bool>.Failed(status.ErrorCode);

            if (status.Data != OrderStatus.BuyerPending)
                return RequestResult<bool>.Failed(ErrorCode.InvalidOrderStatusTransition);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var payment = await _mediator.Send(
                    new CreatePaymentCommand(request.UserId, request.OrderId, request.Method),
                    cancellationToken);

                if (!payment.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RequestResult<bool>.Failed(payment.ErrorCode);
                }

                var accepted = await _mediator.Send(
                    new AcceptOrderQuoteCommand(request.UserId, request.OrderId), cancellationToken);

                if (!accepted.IsSuccess)
                {
                    // The charge went through but the order would not move. Undo the
                    // charge before unwinding, so the refund is recorded either way.
                    await _mediator.Send(
                        new RefundPaymentCommand(request.OrderId, "Order could not be confirmed after payment."),
                        cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                    return RequestResult<bool>.Failed(accepted.ErrorCode);
                }

                await transaction.CommitAsync(cancellationToken);

                return RequestResult<bool>.Success(true);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
