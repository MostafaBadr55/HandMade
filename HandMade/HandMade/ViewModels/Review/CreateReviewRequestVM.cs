using HandMade.Domain.DomainEnums;
using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Review
{
    public class CreateReviewRequestVM
    {
        /// <summary>
        /// Buyer reviews are written by artists about clients, so a client may only
        /// review a Product or a Shop here.
        /// </summary>
        [AllowedValues(ReviewTargetType.Product, ReviewTargetType.Shop,
            ErrorMessage = "Target type must be Product or Shop")]
        public ReviewTargetType TargetType { get; set; }

        [Required(ErrorMessage = "Target id is required")]
        public Guid TargetId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
    }
}
