namespace HandMade.ViewModels.StoreFront
{
    public class HomePageResponseVM
    {
        public List<CategoryCardResponseVM> Categories { get; set; } = [];
        public List<ShopCardResponseVM> TopRatedShops { get; set; } = [];
        public List<ProductCardResponseVM> MostRecentProducts { get; set; } = [];
    }
}
