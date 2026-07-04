using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetPublicProductDetails.DTOs
{
    public class PublicProductReviewDTO
    {
        public string ReviewerName { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewContent { get; set; }
        public int Rating { get; set; }
    }
}
