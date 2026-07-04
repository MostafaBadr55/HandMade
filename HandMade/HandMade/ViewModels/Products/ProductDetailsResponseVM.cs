
using HandMade.ViewModels.ProductImage;
using HandMade.ViewModels.Review;

namespace HandMade.ViewModels.Products
{
    public class ProductDetailsResponseVM
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
        public List<ProductReviewResonseVM> Reviews { get; set; } = [];
        public List<ProductImageResponseVM> Images { get; set; } = [];
    }
}
