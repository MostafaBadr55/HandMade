using HandMade.Application.Features.ProductImages.Commands.UpdateProductImages.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Products
{
    public record UpdateProductMainInfoRequestVM(
    [Required] string Title,
    [Required] Guid CategoryId,
    [Required] Guid SubCategoryId,
    [Required] List<UpdateProductImageDTO> Images);
}
