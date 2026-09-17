using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.Orders.Responses
{
    public class ShopOrderListItemResponseVM
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public Guid BuyerUserId { get; set; }
        public string BuyerUserName { get; set; }
        public string ProductTitle { get; set; }
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal GrandTotal { get; set; }
        public int? ExecutionDays { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
