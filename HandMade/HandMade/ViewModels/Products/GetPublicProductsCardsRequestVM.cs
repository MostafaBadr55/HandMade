using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Shared;

namespace HandMade.ViewModels.Products
{
    public class GetPublicProductsCardsRequestVM
    {
        public Guid? ShopId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubCategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public PublicProductSortBy? SortBy { get; set; }
        public SortDirection? SortDirection { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
