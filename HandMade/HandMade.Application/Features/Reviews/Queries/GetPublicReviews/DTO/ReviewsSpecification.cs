using HandMade.Application.Features.Reviews.Queries.GetPublicReviews.DTO;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Reviews.Queries.GetPublicReviews.DTO
{
    public class ReviewsSpecification : BaseSpecification<Review>
    {
        public ReviewsSpecification( ReviewsCriteria criteria)
        {
            Criteria = r =>
                (!criteria.TargetType.HasValue||r.TargetType == criteria.TargetType) &&
                (!criteria.TargetId.HasValue||r.TargetId == criteria.TargetId) &&
                (!criteria.Status.HasValue || r.Status == criteria.Status) &&
                (!criteria.RatingFilter.HasValue || r.Rating == criteria.RatingFilter.Value);

            switch (criteria.SortBy)
            {
                case ReviewSortBy.RatingAscending:
                    OrderBy = r => r.Rating;
                    break;
                case ReviewSortBy.NewestFirst:
                    OrderByDescending = r => r.CreatedAt;
                    break;
                case ReviewSortBy.OldestFirst:
                    OrderBy = r => r.CreatedAt;
                    break;
                case ReviewSortBy.RatingDescending:
                default:
                    OrderByDescending = r => r.Rating;
                    break;
            }
        }
    }
}
