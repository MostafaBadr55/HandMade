using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.RejectOrderQuote
{
    /// <summary>
    /// BuyerPending -> Cancelled. The negotiation is single-round: to try again the
    /// client raises a fresh request rather than counter-offering on this one.
    /// </summary>
    public record RejectOrderQuoteCommand(
        Guid UserId,
        Guid OrderId,
        string? Reason) : IRequest<RequestResult<bool>>;

    public class RejectOrderQuoteCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<RejectOrderQuoteCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            RejectOrderQuoteCommand request,
            CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();

            var order = await _executor.FirstOrDefaultAsync(
                orderRepo.GetByIdWithTracking(request.OrderId),
                cancellationToken);

            if (order is null)
                return RequestResult<bool>.Failed(ErrorCode.OrderNotFound);

            if (order.UserId != request.UserId)
                return RequestResult<bool>.Failed(ErrorCode.OrderAccessDenied);

            if (order.Status != OrderStatus.BuyerPending)
                return RequestResult<bool>.Failed(ErrorCode.InvalidOrderStatusTransition);

            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = request.Reason;
            order.CancelledByUserId = request.UserId;
            order.CancelledAt = DateTime.UtcNow;

            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
