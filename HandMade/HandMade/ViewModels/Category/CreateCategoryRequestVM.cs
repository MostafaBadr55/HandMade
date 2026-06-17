using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Category
{
    public class CreateCategoryRequestVM
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}
