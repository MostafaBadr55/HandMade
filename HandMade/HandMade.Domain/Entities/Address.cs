using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HandMade.Domain.Entities
{
    public class Address: BaseModel
    {
        public Guid UserId { get; set; }

        [MaxLength(100)]
        public string Label { get; set; }

        [Required]
        [MaxLength(200)]
        public string DetailedAddress { get; set; }

        public bool IsDefault { get; set; } = false;

        // Navigation Properties
        public ICollection<Order> Orders { get; set; }
        public ICollection<Shipment> Shipments { get; set; }
    }
}
