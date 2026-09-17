using HandMade.Domain.DomainEnums;

namespace HandMade.Application.Features.Orders.Queries.GetShopOrderDetails.DTOs
{
    /// <summary>
    /// The artist's view of one order — the mirror of MyOrderDetailsDTO, minus the
    /// buyer's own payment/escrow internals (the artist doesn't need to see the
    /// PaymentStatus row, only that the order has moved to InProgress or beyond).
    /// </summary>
    public class ShopOrderDetailsDTO
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatus Status { get; set; }

        public Guid BuyerUserId { get; set; }
        public string BuyerUserName { get; set; } = string.Empty;

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

        public List<string> AttachmentUrls { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
