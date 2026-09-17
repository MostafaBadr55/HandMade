using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.RejectOrderRequest
{
    /// <summary>
    /// SellerPending -> Cancelled. The artist declines a request before ever
    /// quoting it. No money has moved yet, so this is a plain cancellation, not a
    /// refund.
    /// </summary>
    public record RejectOrderRequestCommand(
        Guid UserId,
        Guid OrderId,
        string? Reason) : IRequest<RequestResult<bool>>;

    public class RejectOrderRequestCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<RejectOrderRequestCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            RejectOrderRequestCommand request,
            CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();

            var order = await _executor.FirstOrDefaultAsync(
                orderRepo.GetByIdWithTracking(request.OrderId),
                cancellationToken);

            if (order is null)
                return RequestResult<bool>.Failed(ErrorCode.OrderNotFound);

            var isOwner = await _unitOfWork.GetRepository<Shop>()
                .AnyAsync(s => s.Id == order.ShopId && s.OwnerUserId == request.UserId, cancellationToken);

            if (!isOwner)
                return RequestResult<bool>.Failed(ErrorCode.OrderAccessDenied);

            if (order.Status != OrderStatus.SellerPending)
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
