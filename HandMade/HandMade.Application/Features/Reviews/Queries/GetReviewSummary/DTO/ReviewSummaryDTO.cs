using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Reviews.Queries.GetReviewSummary.DTO
{
    public class ReviewSummaryDTO
    {
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
