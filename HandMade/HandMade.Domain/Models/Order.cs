using ArtEva.DTOs.Order;
using ArtEva.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArteEva.Models
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
        public int ? CancelledByUserId { get; set; }
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

    public static Order CreateFrom(CreateOrderFromCartItemDto data,PricingResult pricing,string orderNumber)
        {
            return new Order
            {
                UserId = data.UserId,
                CartItemId = data.CartItemId,

                ProductId = data.ProductId,
                ShopId = data.ShopId,

                Quantity = data.Quantity,

                UnitPriceSnapshot = data.UnitPrice,
                ProductTitleSnapshot = data.ProductTitle,
                ProductImageSnapshot = data.ProductImage,

                Subtotal = data.Subtotal,
                ShippingFee = pricing.ShippingFee,
                TaxTotal = pricing.TaxTotal,
                GrandTotal = pricing.GrandTotal,

                ExecutionDays = data.ExecutionDays,
                Status = OrderStatus.SellerPending,

                OrderNumber = orderNumber,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
