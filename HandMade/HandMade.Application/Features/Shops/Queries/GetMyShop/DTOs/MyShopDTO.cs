using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs;
using HandMade.Domain.DomainEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetMyShop.DTOs
{
    public class MyShopDTO
    {
        public Guid Id { get; set; }
        public string OwnerUserName { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public ShopStatus Status { get; set; }
        public decimal RatingAverage { get; set; }
        public string? RejectionMessage { get; set; }

        public List<ProductForSellerDTO> ActiveProductDtos { get; set; }
        public List<ProductForSellerDTO> InActiveProductDtos { get; set; }
    }
}
