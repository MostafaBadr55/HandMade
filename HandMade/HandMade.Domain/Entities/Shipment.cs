using HandMade.Domain.DomainEnums;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
 

    public class Shipment : BaseModel
    {
        public Guid OrderId { get; set; }
        public Guid ShippingAddressId { get; set; }

        [MaxLength(100)]
        public string Carrier { get; set; }

        [MaxLength(100)]
        public string TrackingNumber { get; set; }

        [MaxLength(50)]
        public string RecipientCodename { get; set; }

        public string PickupInstructions { get; set; }

        public string DeliveryNotes { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime? PickedUpAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
        public Address ShippingAddress { get; set; }
    }
}
