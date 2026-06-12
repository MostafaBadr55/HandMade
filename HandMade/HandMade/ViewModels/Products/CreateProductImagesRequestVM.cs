namespace HandMade.ViewModels.Products
{
    public class CreateProductImagesRequestVM
    {
        public string RelativePath { get; set; }
        public string AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}
