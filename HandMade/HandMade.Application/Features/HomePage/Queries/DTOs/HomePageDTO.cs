using HandMade.Application.Features.Categories.Queries.GetCategoryCards.DTOs;
using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Features.Shops.Queries.GetPublicShopCards.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.HomePage.Queries.DTOs
{
    public class HomePageDTO
    {
        public List<CategoryCardDTO> Categories { get; set; } = [];
        public List<PublicShopCardDTO> TopRatedShops { get; set; } = [];
        public List<PublicProductCardDTO> MostRecentProducts { get; set; } = [];
    }
}
