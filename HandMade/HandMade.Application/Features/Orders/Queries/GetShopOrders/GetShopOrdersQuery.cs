using HandMade.Application.Features.Orders.Queries.GetShopOrders.DTOs;
using HandMade.Application.Features.Orders.Queries.GetShopOrders.FilterHelpers;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Queries.GetShopOrders
{
    /// <summary>
    /// The artist's incoming orders — the mirror of GetMyOrdersQuery, filtered by
    /// the caller's shop (Shop.OwnerUserId) instead of the buyer's UserId.
    /// </summary>
    public record GetShopOrdersQuery(
        Guid UserId,
        ShopOrdersCriteria Criteria,
        int PageNumber,
        int PageSize) : IRequest<RequestResult<PagedResult<ShopOrderListItemDTO>>>;

    public class GetShopOrdersQueryHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IUrlBuilder _urlBuilder,
        IAccountServices _accountServices)
        : IRequestHandler<GetShopOrdersQuery, RequestResult<PagedResult<ShopOrderListItemDTO>>>
    {
        public async Task<RequestResult<PagedResult<ShopOrderListItemDTO>>> Handle(
            GetShopOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new ShopOrdersSpecification(request.UserId, request.Criteria);

            var paged = await _unitOfWork
                .GetRepository<Order>()
                .GetAll()
                .ApplySpecification(spec)
                .Select(o => new ShopOrderListItemDTO
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status,
                    BuyerUserId = o.UserId,
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

            if (paged.Items.Count > 0)
            {
                // Single batch lookup — one DB call for all buyer IDs on this page.
                var buyerIds = paged.Items.Select(o => o.BuyerUserId).Distinct();
                var usernameMap = await _accountServices.GetUsernamesByIdsAsync(buyerIds);

                foreach (var order in paged.Items)
                    order.BuyerUserName = usernameMap.GetValueOrDefault(order.BuyerUserId, string.Empty);
            }

            return RequestResult<PagedResult<ShopOrderListItemDTO>>.Success(paged);
        }
    }
}
