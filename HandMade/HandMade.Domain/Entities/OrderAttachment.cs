using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
    /// <summary>
    /// A reference image the client attaches to a custom order request,
    /// so the artist can see what is being asked for before quoting.
    /// Mirrors <see cref="ProductImage"/>: the Url is a root-relative path,
    /// made absolute at read time via IUrlBuilder.
    /// </summary>
    public class OrderAttachment : BaseModel
    {
        public Guid OrderId { get; set; }

        [Required]
        [MaxLength(400)]
        public string Url { get; set; }

        public int SortOrder { get; set; }

        // Navigation Properties
        public Order Order { get; set; }
    }
}
