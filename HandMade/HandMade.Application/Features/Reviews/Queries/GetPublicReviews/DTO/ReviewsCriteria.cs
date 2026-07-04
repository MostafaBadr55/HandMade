using HandMade.Domain.DomainEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Reviews.Queries.GetPublicReviews.DTO
{
    public class ReviewsCriteria
    {
        public ReviewTargetType? TargetType { get; set; }
        public Guid? TargetId { get; set; }
        public ReviewStatus? Status { get; set; }
        public int? RatingFilter { get; set; }
        public ReviewSortBy SortBy { get; set; }
    }
}
