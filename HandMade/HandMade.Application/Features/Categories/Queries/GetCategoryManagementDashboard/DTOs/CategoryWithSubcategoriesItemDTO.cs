using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Queries.GetCategoryManagementDashboard.DTOs
{
    public class CategoryWithSubcategoriesItemDTO
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<SubCategorItemDTO> Subcategories { get; set; }
    }
}
