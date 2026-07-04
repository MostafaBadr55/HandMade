using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Queries.GetProductsPrimaryImage.DTOs
{
    public class PrimaryImageDTO
    {
        public Guid ProductId { get; set; }
        public string RelativePath { get; set; }
        public string AltText { get; set; }
    }
}
