using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Queries.GetOrderStatus
{
    /// <summary>
    /// Cheap status read used to gate a transition before anything expensive or
    /// irreversible happens — notably to refuse an accept before the card is charged.
    /// </summary>
    public record GetOrderStatusQuery(Guid UserId, Guid OrderId) : IRequest<RequestResult<OrderStatus>>;

    public class GetOrderStatusQueryHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<GetOrderStatusQuery, RequestResult<OrderStatus>>
    {
        public async Task<RequestResult<OrderStatus>> Handle(
            GetOrderStatusQuery request,
            CancellationToken cancellationToken)
        {
            var order = await _executor.FirstOrDefaultAsync(
                _unitOfWork.GetRepository<Order>()
                    .Get(o => o.Id == request.OrderId && o.UserId == request.UserId)
                    .Select(o => new { o.Status }),
                cancellationToken);

            if (order is null)
                return RequestResult<OrderStatus>.Failed(ErrorCode.OrderNotFound);

            return RequestResult<OrderStatus>.Success(order.Status);
        }
    }
}
