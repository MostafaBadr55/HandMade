using HandMade.Domain.DomainEnums;
using System;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
  

    public class Refund : BaseModel
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }

        public decimal Amount { get; set; }

        public RefundStatus Status { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
        public Payment Payment { get; set; }
    }
}
