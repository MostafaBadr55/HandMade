namespace HandMade.Domain.Entities
{
    public class CartItem : BaseModel
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }


        // 🆕 Order linking
        public Guid? OrderId { get; set; }
        public bool IsConvertedToOrder { get; set; } = false;

        // Navigation Properties
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}
