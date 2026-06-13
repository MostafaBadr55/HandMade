using HandMade.Domain.DomainEnums;
using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Products
{
    public record UpdateProductStatusRequestVM(
     [Required] ProductStatus Status);
}
