namespace HandMade.Application.Features.Carts.Queries.GetMyCart.DTOs
{
    public class CartItemDTO
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

        /// <summary>
        /// The product may have been unpublished, deactivated or its shop suspended
        /// since it was added. Surfaced so the cart can flag the line before checkout
        /// rather than failing the whole checkout with no explanation.
        /// </summary>
        public bool IsStillPurchasable { get; set; }
    }
}
