using HandMade.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs
{
    public class PublicProductsCriteria
    {
       public Guid? ShopId { get; set; }
       public Guid? CategoryId { get; set; }
       public Guid? SubCategoryId { get; set; }
       public string? SearchTerm { get; set; }
       public PublicProductSortBy? SortBy { get; set; }
       public SortDirection SortDirection { get; set; }
    }

    public enum PublicProductSortBy
    {
        Price,
        CreatedAt
    }

   
}
