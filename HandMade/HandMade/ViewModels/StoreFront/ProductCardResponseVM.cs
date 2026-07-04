namespace HandMade.ViewModels.StoreFront
{
    public class ProductCardResponseVM
    {
        public Guid ProductId { get; set; }
        public Guid ShopId { get; set; }
        public string ProductName { get; set; }
        public string ShopName { get; set; }
        public decimal Price { get; set; }
        public int ExpectedDays { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string? RelativePath { get; set; }
        public string? AltText { get; set; }
    }
}
