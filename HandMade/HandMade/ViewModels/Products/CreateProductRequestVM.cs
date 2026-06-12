namespace HandMade.ViewModels.Products
{
    public class CreateProductRequestVM
    {
        public Guid ShopId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public string  Title { get; set; }
        public decimal Price { get; set; }
        public List<CreateProductImagesRequestVM> Images { get; set; }
    }
}
