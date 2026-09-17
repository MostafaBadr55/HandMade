using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Payments.Commands.CreatePayment
{
    /// <summary>
    /// Charges the client and records the money as held in escrow. The idempotency
    /// key is derived from the order, so a client who submits accept twice is
    /// charged once — the second attempt finds the existing completed payment.
    /// </summary>
    public record CreatePaymentCommand(
        Guid UserId,
        Guid OrderId,
        PaymentMethod Method) : IRequest<RequestResult<Guid>>;

    public class CreatePaymentCommandHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IPaymentGateway _paymentGateway)
        : IRequestHandler<CreatePaymentCommand, RequestResult<Guid>>
    {
        private const string Currency = "EGP";

        public async Task<RequestResult<Guid>> Handle(
            CreatePaymentCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _executor.FirstOrDefaultAsync(
                _unitOfWork.GetRepository<Order>()
                    .GetById(request.OrderId)
                    .Select(o => new { o.Id, o.UserId, o.GrandTotal }),
                cancellationToken);

            if (order is null)
                return RequestResult<Guid>.Failed(ErrorCode.OrderNotFound);

            if (order.UserId != request.UserId)
                return RequestResult<Guid>.Failed(ErrorCode.OrderAccessDenied);

            var paymentRepo = _unitOfWork.GetRepository<Payment>();
            var idempotencyKey = $"order-{request.OrderId}";

            // Only a *live* payment counts. A refunded or failed attempt must never be
            // mistaken for money on the table — reusing one would move the order to
            // InProgress with nothing held in escrow.
            var existing = await _executor.FirstOrDefaultAsync(
                paymentRepo.Get(p => p.IdempotencyKey == idempotencyKey),
                cancellationToken);

            if (existing is not null)
            {
                if (existing.Status == PaymentStatus.Completed)
                    return RequestResult<Guid>.Failed(ErrorCode.PaymentAlreadyCompleted);

                if (existing.Status is PaymentStatus.Pending or PaymentStatus.Processing)
                    return RequestResult<Guid>.Success(existing.Id);

                // Refunded / Failed / Cancelled: the key belongs to a dead attempt.
                // The unique index is filtered on NOT NULL, so releasing it here lets
                // this retry charge again without a second row fighting for the key.
                var dead = await _executor.FirstOrDefaultAsync(
                    paymentRepo.GetByIdWithTracking(existing.Id), cancellationToken);

                if (dead is not null)
                {
                    dead.IdempotencyKey = null;
                    paymentRepo.Update(dead);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            var charge = await _paymentGateway.ChargeAsync(
                order.GrandTotal, Currency, idempotencyKey, cancellationToken);

            if (!charge.Succeeded)
                return RequestResult<Guid>.Failed(ErrorCode.PaymentFailed);

            var payment = new Payment
            {
                OrderId = request.OrderId,
                UserId = request.UserId,
                Amount = order.GrandTotal,
                Currency = Currency,
                Method = request.Method,
                Status = PaymentStatus.Completed,
                EscrowStatus = EscrowStatus.Held,
                ProviderRef = charge.ProviderRef!,
                IdempotencyKey = idempotencyKey,
                PaidAt = DateTime.UtcNow
            };

            paymentRepo.Add(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<Guid>.Success(payment.Id);
        }
    }
}
