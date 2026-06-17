using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Queries.GetCategoryManagementDashboard.DTOs
{
    public class SubCategorItemDTO
    {
        public Guid Id { get; set; }
        public string SubcategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
