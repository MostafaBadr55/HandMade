using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;

namespace HandMade.Application.Features.Orders.Queries.GetShopOrders.FilterHelpers
{
    public class ShopOrdersCriteria
    {
        public OrderStatus? Status { get; set; }
        public ShopOrderSortBy SortBy { get; set; } = ShopOrderSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
