using HandMade.Application.Features.Products.Queries.FilterHelpers;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs
{
    public class ProductsForSellerCriteria
    {
        public ProductStatus? Status { get; set; }
        public ProductApprovalStatus? ApprovalStatus { get; set; }
        public bool? IsPublished { get; set; }
        public ProductSortBy SortBy { get; set; } = ProductSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
    }
}
