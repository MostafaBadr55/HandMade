using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs;
using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.Shop
{
    public class MyShopResponseVM
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
