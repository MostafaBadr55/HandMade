using HandMade.Application.Features.Orders.Queries.GetShopOrders.FilterHelpers;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.Orders.Requests
{
    public class GetShopOrdersRequestVM
    {
        public OrderStatus? Status { get; set; }
        public ShopOrderSortBy SortBy { get; set; } = ShopOrderSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
