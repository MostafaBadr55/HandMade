using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers
{
    public class ShopQueryCriteria
    {
        public Guid? OwnerUserId { get; set; }
        public ShopStatus? Status { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? MaxRating { get; set; }
        public string? Name { get; set; }
        public ShopSortBy SortBy { get; set; } = ShopSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
