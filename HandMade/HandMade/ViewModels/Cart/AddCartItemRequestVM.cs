using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Cart
{
    public class AddCartItemRequestVM
    {
        [Required(ErrorMessage = "Product id is required")]
        public Guid ProductId { get; set; }

        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; } = 1;
    }
}
