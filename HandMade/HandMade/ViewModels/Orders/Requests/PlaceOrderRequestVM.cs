using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Orders.Requests
{
    public class PlaceOrderRequestVM
    {
        [Required(ErrorMessage = "Product id is required")]
        public Guid ProductId { get; set; }

        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Shipping address is required")]
        public Guid ShippingAddressId { get; set; }

        [MaxLength(2000, ErrorMessage = "Special instructions cannot exceed 2000 characters")]
        public string? SpecialInstructions { get; set; }

        /// <summary>
        /// Root-relative paths returned by POST /api/Files/upload.
        /// </summary>
        public List<string> AttachmentPaths { get; set; } = new();
    }
}
