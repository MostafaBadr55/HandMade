using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.SubmitOrderQuote
{
    /// <summary>
    /// SellerPending -> BuyerPending. The artist's half of the single-round
    /// negotiation: sets the price and timeline the client will accept or reject.
    /// There is no offer history — a resubmitted quote would overwrite these same
    /// snapshot fields, but the transition only allows it once per request.
    /// </summary>
    public record SubmitOrderQuoteCommand(
        Guid UserId,
        Guid OrderId,
        decimal Price,
        int ExecutionDays) : IRequest<RequestResult<bool>>;

    public class SubmitOrderQuoteCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<SubmitOrderQuoteCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            SubmitOrderQuoteCommand request,
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

            order.UnitPriceSnapshot = request.Price;
            order.ExecutionDays = request.ExecutionDays;
            order.Subtotal = request.Price * order.Quantity;
            order.GrandTotal = order.Subtotal + order.ShippingFee + order.TaxTotal;
            order.Status = OrderStatus.BuyerPending;

            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
