using HandMade.Domain.DomainEnums;
using System;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
   

    public class Dispute : BaseModel
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public DisputeStatus Status { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
    }
}
