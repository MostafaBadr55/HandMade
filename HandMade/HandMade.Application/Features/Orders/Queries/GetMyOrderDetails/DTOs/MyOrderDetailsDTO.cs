using HandMade.Domain.DomainEnums;

namespace HandMade.Application.Features.Orders.Queries.GetMyOrderDetails.DTOs
{
    public class MyOrderDetailsDTO
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }

        public Guid ShopId { get; set; }
        public string ShopName { get; set; }

        public Guid ProductId { get; set; }
        public string ProductTitleSnapshot { get; set; }
        public string? ProductImageSnapshot { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal GrandTotal { get; set; }

        public int? ExecutionDays { get; set; }
        public string? SpecialInstructions { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? AutoReleaseAt { get; set; }

        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }

        public string? ShippingAddressLabel { get; set; }
        public string? ShippingAddressDetails { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }
        public EscrowStatus? EscrowStatus { get; set; }

        public List<string> AttachmentUrls { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
