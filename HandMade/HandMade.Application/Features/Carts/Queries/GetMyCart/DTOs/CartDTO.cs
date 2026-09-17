namespace HandMade.Application.Features.Carts.Queries.GetMyCart.DTOs
{
    public class CartDTO
    {
        public Guid CartId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
        public int ItemCount { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime? CheckedOutAt { get; set; }
    }
}
