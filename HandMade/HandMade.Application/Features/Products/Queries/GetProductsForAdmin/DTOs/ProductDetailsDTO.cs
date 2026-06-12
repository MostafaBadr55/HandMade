using HandMade.Application.Features.ProductImages.Queries.GetProductImages.DTOs;
using HandMade.Application.Features.ProductImages.Queries.GetProductsImages.DTO;
using HandMade.Domain.DomainEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetProductsForAdmin.DTOs
{
    public class ProductDetailsDTO
    {
        public Guid Id { get; set; }
        public string ShopName { get; set; }
        public bool IsPublished { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public ProductStatus Status { get; set; }
        public List<ProductImageDictionaryItemDTO> Images { get; set; } = new();
    }
}
