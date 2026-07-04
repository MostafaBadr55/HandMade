using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetPublicShopCards.DTOs
{
    public class PublicShopCardDTO
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public double Rating { get; set; } = 0;
        
    }
}
