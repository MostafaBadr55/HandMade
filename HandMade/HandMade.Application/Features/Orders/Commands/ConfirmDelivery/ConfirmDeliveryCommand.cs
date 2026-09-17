using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.ConfirmDelivery
{
    /// <summary>
    /// CompletedBySeller -> Delivered. Reached either by the client confirming, or by
    /// the auto-release sweep once AutoReleaseAt has passed — hence the nullable
    /// ConfirmedByUserId: an automatic confirmation has no acting user.
    /// </summary>
    public record ConfirmDeliveryCommand(Guid OrderId, Guid? ConfirmedByUserId) : IRequest<RequestResult<bool>>;

    public class ConfirmDeliveryCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<ConfirmDeliveryCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ConfirmDeliveryCommand request,
            CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();

            var order = await _executor.FirstOrDefaultAsync(
                orderRepo.GetByIdWithTracking(request.OrderId),
                cancellationToken);

            if (order is null)
                return RequestResult<bool>.Failed(ErrorCode.OrderNotFound);

            if (request.ConfirmedByUserId is not null && order.UserId != request.ConfirmedByUserId)
                return RequestResult<bool>.Failed(ErrorCode.OrderAccessDenied);

            if (order.Status != OrderStatus.CompletedBySeller)
                return RequestResult<bool>.Failed(ErrorCode.InvalidOrderStatusTransition);

            order.Status = OrderStatus.Delivered;

            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
