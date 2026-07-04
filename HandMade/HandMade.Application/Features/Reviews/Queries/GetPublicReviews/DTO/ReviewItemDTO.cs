using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Reviews.Queries.GetPublicReviews.DTO
{
    public class ReviewItemDTO
    {
        public string ReviewerName { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewContent { get; set; }
        public int Rating { get; set; }
    }
}
