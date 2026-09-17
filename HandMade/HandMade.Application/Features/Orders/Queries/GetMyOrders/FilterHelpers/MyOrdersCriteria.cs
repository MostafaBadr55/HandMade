using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;

namespace HandMade.Application.Features.Orders.Queries.GetMyOrders.FilterHelpers
{
    public class MyOrdersCriteria
    {
        public OrderStatus? Status { get; set; }
        public Guid? ShopId { get; set; }
        public MyOrderSortBy SortBy { get; set; } = MyOrderSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
