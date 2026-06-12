using HandMade.Application.Features.ProductImages.Queries.GetProductsImages.DTO;
using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.AdminDashboard.Responses
{
    public class ProductForAdminResponseVM
    {
        public Guid? Id { get; set; }
        public string? ShopName { get; set; }
        public bool? IsPublished { get; set; }
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public ProductStatus? Status { get; set; }
        public List<ProductImageDictionaryItemDTO> Images { get; set; } = new();
    }
}
