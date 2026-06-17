using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.SubCategory
{
    public class UpdateSubCategoryRequestVM
    {
        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
