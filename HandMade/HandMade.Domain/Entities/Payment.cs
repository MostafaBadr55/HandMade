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

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }

        [MaxLength(200)]
        public string ProviderRef { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
        public ICollection<Refund> Refunds { get; set; }
    }
}
