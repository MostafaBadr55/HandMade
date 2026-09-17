using HandMade.Application.Features.Orders.Commands.ConfirmDelivery;
using HandMade.Application.Features.Payments.Commands.ReleaseEscrow;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using MediatR;

namespace HandMade.Application.Features.Orders.Orchestrators.ConfirmDeliveryAction
{
    /// <summary>
    /// Closes the loop: the order is marked Delivered and the escrowed money is
    /// released to the artist, together. Shared by the client-facing confirm endpoint
    /// and the auto-release sweep — pass a null ConfirmedByUserId for the latter.
    /// </summary>
    public record ConfirmDeliveryOrchestrator(
        Guid OrderId,
        Guid? ConfirmedByUserId) : IRequest<RequestResult<bool>>;

    public class ConfirmDeliveryOrchestratorHandler(IMediator _mediator, IUnitOfWork _unitOfWork)
        : IRequestHandler<ConfirmDeliveryOrchestrator, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ConfirmDeliveryOrchestrator request,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var confirmed = await _mediator.Send(
                    new ConfirmDeliveryCommand(request.OrderId, request.ConfirmedByUserId), cancellationToken);

                if (!confirmed.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RequestResult<bool>.Failed(confirmed.ErrorCode);
                }

                var released = await _mediator.Send(
                    new ReleaseEscrowCommand(request.OrderId), cancellationToken);

                // An order confirmed without releasing the money would strand the
                // artist, so the pair is all-or-nothing.
                if (!released.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RequestResult<bool>.Failed(released.ErrorCode);
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
