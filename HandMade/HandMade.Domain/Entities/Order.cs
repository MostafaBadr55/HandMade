using HandMade.Domain.DomainEnums;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
    public class Order : BaseModel
    {

        public int? CartItemId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int ShopId { get; set; }
        public Shop Shop { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int? ShippingAddressId { get; set; }
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
        public DateTime? ConfirmedAt { get; set; }
        public string? CancellationReason { get; set; }
        public int? CancelledByUserId { get; set; }
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
