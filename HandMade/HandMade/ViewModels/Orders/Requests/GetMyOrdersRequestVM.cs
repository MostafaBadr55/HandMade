using HandMade.Application.Features.Orders.Queries.GetMyOrders.FilterHelpers;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.Orders.Requests
{
    public class GetMyOrdersRequestVM
    {
        public OrderStatus? Status { get; set; }
        public Guid? ShopId { get; set; }
        public MyOrderSortBy SortBy { get; set; } = MyOrderSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
