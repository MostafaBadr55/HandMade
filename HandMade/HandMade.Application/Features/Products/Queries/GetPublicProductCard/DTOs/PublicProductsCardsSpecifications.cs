using HandMade.Application.Features.Products.Queries.FilterHelpers;
using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs
{
    public class PublicProductsCardsSpecifications : BaseSpecification<Product>
    {
        public PublicProductsCardsSpecifications(PublicProductsCriteria criteria)
        {
            Criteria = p =>
            p.IsPublished &&
            p.ApprovalStatus == ProductApprovalStatus.Approved &&
            (criteria.ShopId == null || p.ShopId == criteria.ShopId) &&
            (criteria.CategoryId == null || p.CategoryId == criteria.CategoryId) &&
            (criteria.SubCategoryId == null || p.SubCategoryId == criteria.SubCategoryId) &&
            (criteria.SearchTerm == null || p.Title.Contains(criteria.SearchTerm));

            ApplySorting(criteria);
        }

             private void ApplySorting(PublicProductsCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case PublicProductSortBy.Price:
                    ApplyPriceSorting(criteria.SortDirection);
                    break;

                default:
                    ApplyCreatedAtSorting(criteria.SortDirection);
                    break;
            }
        }

        private void ApplyPriceSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = p => p.Price;
            else
                OrderByDescending = p => p.Price;
        }
        private void ApplyCreatedAtSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = p => p.CreatedAt;
            else
                OrderByDescending = p => p.CreatedAt;
        }

    }
}
