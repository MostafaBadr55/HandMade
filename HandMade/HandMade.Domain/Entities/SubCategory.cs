using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
    public class SubCategory : BaseModel
    {
        public Guid CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation Properties
        public Category Category { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
