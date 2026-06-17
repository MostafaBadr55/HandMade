using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Shop
{
    public class CreateShopRequestVM
    {

        [Required(ErrorMessage = "Shop name is required")]
        [MaxLength(120, ErrorMessage = "Shop name cannot exceed 120 characters")]
        public string ShopName { get; set; }
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
        public string imagePath { get; set; }
    }
}
