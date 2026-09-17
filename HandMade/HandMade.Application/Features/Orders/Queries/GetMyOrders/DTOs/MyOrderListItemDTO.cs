using HandMade.Domain.DomainEnums;

namespace HandMade.Application.Features.Orders.Queries.GetMyOrders.DTOs
{
    public class MyOrderListItemDTO
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public Guid ShopId { get; set; }
        public string ShopName { get; set; }
        public string ProductTitleSnapshot { get; set; }
        public string? ProductImageSnapshot { get; set; }
        public int Quantity { get; set; }
        public decimal GrandTotal { get; set; }
        public int? ExecutionDays { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
