using HandMade.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetPublicShopCards.DTOs
{
    public class PublicShopCriteria
    {
        public string ShopName { get; set; }
        public Guid CategoryId { get; set; }
        public PublicShopSortBy SortBy { get; set; }
        public SortDirection Direction { get; set; }
        
    }

    public enum PublicShopSortBy
    {
        Name, Rating
    }
}
