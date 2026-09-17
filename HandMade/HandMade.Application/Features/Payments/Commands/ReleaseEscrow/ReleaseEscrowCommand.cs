using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Payments.Commands.ReleaseEscrow
{
    /// <summary>
    /// Hands the held funds to the artist. Both the buyer confirming delivery and the
    /// auto-release sweep reach this, so an already-released payment is reported as
    /// such rather than released twice.
    /// </summary>
    public record ReleaseEscrowCommand(Guid OrderId) : IRequest<RequestResult<bool>>;

    public class ReleaseEscrowCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<ReleaseEscrowCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ReleaseEscrowCommand request,
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

            if (payment.EscrowStatus != EscrowStatus.Held)
                return RequestResult<bool>.Failed(ErrorCode.EscrowNotHeld);

            payment.EscrowStatus = EscrowStatus.Released;
            payment.EscrowReleasedAt = DateTime.UtcNow;

            paymentRepo.Update(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
