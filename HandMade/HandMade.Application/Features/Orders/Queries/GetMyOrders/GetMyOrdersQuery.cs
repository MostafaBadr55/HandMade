using HandMade.Application.Features.Orders.Queries.GetMyOrders.DTOs;
using HandMade.Application.Features.Orders.Queries.GetMyOrders.FilterHelpers;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Queries.GetMyOrders
{
    public record GetMyOrdersQuery(
        Guid UserId,
        MyOrdersCriteria Criteria,
        int PageNumber,
        int PageSize) : IRequest<RequestResult<PagedResult<MyOrderListItemDTO>>>;

    public class GetMyOrdersQueryHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IUrlBuilder _urlBuilder)
        : IRequestHandler<GetMyOrdersQuery, RequestResult<PagedResult<MyOrderListItemDTO>>>
    {
        public async Task<RequestResult<PagedResult<MyOrderListItemDTO>>> Handle(
            GetMyOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new MyOrdersSpecification(request.UserId, request.Criteria);

            var paged = await _unitOfWork
                .GetRepository<Order>()
                .GetAll()
                .ApplySpecification(spec)
                .Select(o => new MyOrderListItemDTO
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status,
                    ShopId = o.ShopId,
                    ShopName = o.Shop.Name,
                    ProductTitleSnapshot = o.ProductTitleSnapshot,
                    ProductImageSnapshot = o.ProductImageSnapshot,
                    Quantity = o.Quantity,
                    GrandTotal = o.GrandTotal,
                    ExecutionDays = o.ExecutionDays,
                    ConfirmedAt = o.ConfirmedAt,
                    CreatedAt = o.CreatedAt
                })
                .ToPagedResultAsync(_executor, request.PageNumber, request.PageSize, cancellationToken);

            foreach (var order in paged.Items)
            {
                // ExpectedDeliveryDate is a computed property on the entity, so it
                // cannot be translated inside the projection — derive it here.
                order.ExpectedDeliveryDate = order.ConfirmedAt?.AddDays(order.ExecutionDays ?? 1);

                order.ProductImageSnapshot = order.ProductImageSnapshot is null
                    ? null
                    : _urlBuilder.BuildAbsoluteUrl(order.ProductImageSnapshot);
            }

            return RequestResult<PagedResult<MyOrderListItemDTO>>.Success(paged);
        }
    }
}
