using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers
{
    public class ShopQuerySpecification : BaseSpecification<Shop>
    {
        public ShopQuerySpecification(ShopQueryCriteria criteria)
        {
            Criteria = s =>
                (!criteria.OwnerUserId.HasValue || s.OwnerUserId == criteria.OwnerUserId.Value) &&
                (!criteria.Status.HasValue || s.Status == criteria.Status.Value) &&
                (!criteria.MinRating.HasValue || s.RatingAverage >= criteria.MinRating.Value) &&
                (!criteria.MaxRating.HasValue || s.RatingAverage <= criteria.MaxRating.Value) &&
                (string.IsNullOrEmpty(criteria.Name) || s.Name.Contains(criteria.Name));

            ApplySorting(criteria);
        }

        private void ApplySorting(ShopQueryCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case ShopSortBy.Name:
                    ApplyNameSorting(criteria.SortDirection);
                    break;

                case ShopSortBy.Rating:
                    ApplyRatingSorting(criteria.SortDirection);
                    break;

                default:
                    ApplyCreatedAtSorting(criteria.SortDirection);
                    break;
            }
        }

        private void ApplyNameSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = s => s.Name;
            else
                OrderByDescending = s => s.Name;
        }

        private void ApplyRatingSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = s => s.RatingAverage;
            else
                OrderByDescending = s => s.RatingAverage;
        }

        private void ApplyCreatedAtSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = s => s.CreatedAt;
            else
                OrderByDescending = s => s.CreatedAt;
        }
    }
}
