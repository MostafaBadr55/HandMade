using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Payments.Commands.RefundPayment
{
    /// <summary>
    /// Returns held funds to the client. Used to unwind a charge when a later step of
    /// the accept orchestrator fails — money must never sit in escrow against an
    /// order that did not actually move forward.
    /// </summary>
    public record RefundPaymentCommand(Guid OrderId, string Reason) : IRequest<RequestResult<bool>>;

    public class RefundPaymentCommandHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IPaymentGateway _paymentGateway)
        : IRequestHandler<RefundPaymentCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            RefundPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var paymentRepo = _unitOfWork.GetRepository<Payment>();

            var payment = await _executor.FirstOrDefaultAsync(
                paymentRepo.GetRangeWithTracking(p =>
                    p.OrderId == request.OrderId && p.Status == PaymentStatus.Completed),
                cancellationToken);

            if (payment is null)
                return RequestResult<bool>.Failed(ErrorCode.PaymentNotFound);

            if (payment.EscrowStatus == EscrowStatus.Released)
                return RequestResult<bool>.Failed(ErrorCode.EscrowAlreadyReleased);

            var refund = await _paymentGateway.RefundAsync(
                payment.ProviderRef, payment.Amount, cancellationToken);

            if (!refund.Succeeded)
                return RequestResult<bool>.Failed(ErrorCode.PaymentFailed);

            payment.Status = PaymentStatus.Refunded;
            payment.EscrowStatus = EscrowStatus.Refunded;
            paymentRepo.Update(payment);

            _unitOfWork.GetRepository<Refund>().Add(new Refund
            {
                OrderId = request.OrderId,
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Status = RefundStatus.Completed,
                Reason = request.Reason
            });

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
