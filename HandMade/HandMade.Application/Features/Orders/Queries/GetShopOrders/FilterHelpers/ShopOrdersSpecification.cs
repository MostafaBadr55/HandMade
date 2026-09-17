using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;

namespace HandMade.Application.Features.Orders.Queries.GetShopOrders.FilterHelpers
{
    public class ShopOrdersSpecification : BaseSpecification<Order>
    {
        public ShopOrdersSpecification(Guid ownerUserId, ShopOrdersCriteria criteria)
        {
            Criteria = o =>
                o.Shop.OwnerUserId == ownerUserId &&
                (criteria.Status == null || o.Status == criteria.Status);

            ApplySorting(criteria);
        }

        private void ApplySorting(ShopOrdersCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case ShopOrderSortBy.GrandTotal:
                    Assign(criteria.SortDirection, o => o.GrandTotal);
                    break;

                case ShopOrderSortBy.Status:
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
