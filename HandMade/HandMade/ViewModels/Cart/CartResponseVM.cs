namespace HandMade.ViewModels.Cart
{
    public class CartResponseVM
    {
        public Guid CartId { get; set; }
        public List<CartItemResponseVM> Items { get; set; } = new();
        public int ItemCount { get; set; }
        public decimal Subtotal { get; set; }
    }
}
