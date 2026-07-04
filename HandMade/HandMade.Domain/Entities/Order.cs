using HandMade.Domain.DomainEnums;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
    public class Order : BaseModel
    {

        public Guid? CartItemId { get; set; }
        public Guid UserId { get; set; }

        public Guid ShopId { get; set; }
        public Shop Shop { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }

        [Required, MaxLength(40)]
        public string OrderNumber { get; set; }

        public OrderStatus Status { get; set; }
        public int Quantity { get; set; }

        // Snapshots
        public decimal UnitPriceSnapshot { get; set; }
        [MaxLength(200)]
        public string ProductTitleSnapshot { get; set; }
        public string? ProductImageSnapshot { get; set; }

        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; } = 15;
        public decimal TaxTotal { get; set; }
        public decimal GrandTotal { get; set; }

        // Negotiation Logic
        public int? ExecutionDays { get; set; }
        public string? SpecialInstructions { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? CancellationReason { get; set; }
        public Guid? CancelledByUserId { get; set; }
        public DateTime? CancelledAt { get; set; }

        // Calculated Helper
        public DateTime? ExpectedDeliveryDate => ConfirmedAt?.AddDays(ExecutionDays ?? 1);

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<Payment> Payments { get; set; }
        //public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Shipment> Shipments { get; set; }
        public ICollection<Refund> Refunds { get; set; }
        public ICollection<Dispute> Disputes { get; set; }

    }
}
