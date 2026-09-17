namespace HandMade.ViewModels.Cart
{
    public class CartItemResponseVM
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ShopId { get; set; }
        public string ShopName { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int ExpectedDays { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsStillPurchasable { get; set; }
    }
}
