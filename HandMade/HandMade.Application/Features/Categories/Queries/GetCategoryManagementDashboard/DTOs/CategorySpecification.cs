using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Queries.GetCategoryManagementDashboard.DTOs
{
    public class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification(string? searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
                Criteria = c => c.Name.Contains(searchTerm);

            OrderBy = c => c.Name;
        }
    }
}
