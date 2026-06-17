namespace HandMade.ViewModels.Shop
{
    public class UpdateShopInfoRequest
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopDescription { get; set; }
        public string ImageRelativePath { get; set; }
    }
}
