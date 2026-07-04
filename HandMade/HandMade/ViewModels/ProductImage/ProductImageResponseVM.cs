namespace HandMade.ViewModels.ProductImage
{
    public class ProductImageResponseVM
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }
}
