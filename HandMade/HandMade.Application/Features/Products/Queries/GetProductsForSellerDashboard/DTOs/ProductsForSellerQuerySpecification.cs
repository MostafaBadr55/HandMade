using HandMade.Application.Features.Products.Queries.FilterHelpers;
using HandMade.Application.Features.Products.Queries.GetProductsForAdmin.DTOs;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs
{
    public class ProductsForSellerQuerySpecification: BaseSpecification<Product>
    {
        public ProductsForSellerQuerySpecification(ProductsForSellerCriteria criteria)
        {
            Criteria = p =>
                (!criteria.Status.HasValue || p.Status == criteria.Status.Value) &&
                (!criteria.ApprovalStatus.HasValue || p.ApprovalStatus == criteria.ApprovalStatus.Value) &&
                (!criteria.IsPublished.HasValue || p.IsPublished == criteria.IsPublished.Value);
            ApplySorting(criteria);
        }

        private void ApplySorting(ProductsForSellerCriteria criteria)
        {
            switch (criteria.SortBy)
            {
                case ProductSortBy.Price:
                    ApplyPriceSorting(criteria.SortDirection);
                    break;

                case ProductSortBy.Title:
                    ApplyTitleSorting(criteria.SortDirection);
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

        private void ApplyTitleSorting(SortDirection direction)
        {
            if (direction == SortDirection.Asc)
                OrderBy = p => p.Title;
            else
                OrderByDescending = p => p.Title;
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

