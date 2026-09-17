using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;

namespace HandMade.Application.Features.Orders.Queries.GetMyOrders.FilterHelpers
{
    public class MyOrdersSpecification : BaseSpecification<Order>
    {
        public MyOrdersSpecification(Guid userId, MyOrdersCriteria criteria)
        {
            Criteria = o =>
                o.UserId == userId &&
                (criteria.Status == null || o.Status == criteria.Status) &&
                (criteria.ShopId == null || o.ShopId == criteria.ShopId);

            ApplySorting(criteria);
        }

        private void ApplySorting(MyOrdersCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case MyOrderSortBy.GrandTotal:
                    Assign(criteria.SortDirection, o => o.GrandTotal);
                    break;

                case MyOrderSortBy.Status:
                    Assign(criteria.SortDirection, o => o.Status);
                    break;

                default:
                    Assign(criteria.SortDirection, o => o.CreatedAt);
                    break;
            }
        }

        private void Assign(SortDirection direction, System.Linq.Expressions.Expression<Func<Order, object>> selector)
        {
            if (direction == SortDirection.Asc)
                OrderBy = selector;
            else
                OrderByDescending = selector;
        }
    }
}
