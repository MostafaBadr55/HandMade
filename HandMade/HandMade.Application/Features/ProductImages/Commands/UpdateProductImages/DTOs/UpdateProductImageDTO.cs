using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Commands.UpdateProductImages.DTOs
{
    public class UpdateProductImageDTO
    {
        public string Url { get; set; }
        public string AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}
