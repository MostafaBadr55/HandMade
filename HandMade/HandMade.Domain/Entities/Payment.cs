using HandMade.Domain.DomainEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
    public class Payment : BaseModel
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "EGP";

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }

        /// <summary>
        /// Whether the platform is still holding these funds. Distinct from
        /// <see cref="Status"/>: a payment can be Completed at the gateway
        /// while the money is still Held from the artist.
        /// </summary>
        public EscrowStatus EscrowStatus { get; set; }

        [MaxLength(200)]
        public string ProviderRef { get; set; }

        /// <summary>
        /// Guards against double-charging when a client retries an accept.
        /// Unique — the gateway is passed this same value.
        /// </summary>
        [MaxLength(100)]
        public string? IdempotencyKey { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? EscrowReleasedAt { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
        public ICollection<Refund> Refunds { get; set; }
    }
}
