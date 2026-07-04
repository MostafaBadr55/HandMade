using HandMade.Application.Features.ProductImages.Queries.GetProductImages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetPublicProductDetails.DTOs
{
    public class PublicProductDetailsDTO
    {
        public Guid ProductId { get; set; }
        public Guid ShopId { get; set; }
        public string ShopName { get; set; }
        public string ProductName { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public decimal Price { get; set; }
        public int ExpectedDays { get; set; }
        public string Description { get; set; }
        public List<PublicProductReviewDTO> Reviews { get; set; } = [];
        public List<ProductImageDTO> Images { get; set; } = [];

    }
}
