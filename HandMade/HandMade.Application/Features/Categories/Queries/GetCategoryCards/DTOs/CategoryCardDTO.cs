using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Queries.GetCategoryCards.DTOs
{
    public class CategoryCardDTO
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryImage { get; set; }
    }
}
