using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Queries.GetProductsImages.DTO
{
    public class ProductImageDictionaryItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }
}
