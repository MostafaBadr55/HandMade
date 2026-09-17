using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.CancelOrder
{
    /// <summary>
    /// Walks away before any money changes hands. Allowed only while the order is
    /// still being negotiated — once it is InProgress the artist may already be
    /// working, so unwinding it is a refund/dispute, not a cancellation.
    /// </summary>
    public record CancelOrderCommand(
        Guid UserId,
        Guid OrderId,
        string? Reason) : IRequest<RequestResult<bool>>;

    public class CancelOrderCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<CancelOrderCommand, RequestResult<bool>>
    {
        private static readonly OrderStatus[] CancellableStatuses =
            [OrderStatus.SellerPending, OrderStatus.BuyerPending];

        public async Task<RequestResult<bool>> Handle(
            CancelOrderCommand request,
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

            if (!CancellableStatuses.Contains(order.Status))
                return RequestResult<bool>.Failed(ErrorCode.OrderCannotBeCancelled);

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
