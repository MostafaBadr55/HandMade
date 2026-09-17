using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Orders.Requests
{
    public class CheckoutCartRequestVM
    {
        [Required(ErrorMessage = "Shipping address is required")]
        public Guid ShippingAddressId { get; set; }

        [MaxLength(2000, ErrorMessage = "Special instructions cannot exceed 2000 characters")]
        public string? SpecialInstructions { get; set; }
    }
}
