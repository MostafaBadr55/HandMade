using HandMade.Domain.DomainEnums;
using System;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
   
    public class Review : BaseModel
    {
        // The user who wrote the review (buyer or seller)
        public Guid ReviewerUserId { get; set; }

        // Identify what is being reviewed: Product, Shop, or Buyer
        public ReviewTargetType TargetType { get; set; }

        // This will refer to: ProductId, ShopId, or BuyerId depending on TargetType
        public Guid TargetId { get; set; }

        // Rating (e.g., 1�5)
        public int Rating { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        public ReviewStatus Status { get; set; }

        // Navigation: the writer of the review
        //public User Reviewer { get; set; }

        // NOTE: TargetId is deliberately NOT a mapped relationship.
        // It points at a Product, a Shop or a buyer depending on TargetType, and a
        // single column cannot carry three foreign keys — SQL Server enforces every
        // one of them, so any review would violate the two that do not apply.
        // Queries join manually and MUST filter on TargetType.
    }

}
