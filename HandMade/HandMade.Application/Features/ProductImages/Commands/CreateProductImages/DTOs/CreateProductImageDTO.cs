using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Commands.CreateProductImages.DTOs
{
    public record CreateProductImageDto(string RelativePath, string AltText, bool IsPrimary, int SortOrder);
}
