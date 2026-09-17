using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Address
{
    public class UpdateAddressRequestVM
    {
        [Required(ErrorMessage = "Label is required")]
        [MaxLength(100, ErrorMessage = "Label cannot exceed 100 characters")]
        public string Label { get; set; }

        [Required(ErrorMessage = "Detailed address is required")]
        [MaxLength(200, ErrorMessage = "Detailed address cannot exceed 200 characters")]
        public string DetailedAddress { get; set; }
    }
}
