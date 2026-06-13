using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Products
{
    public record UpdateProductPriceRequestVM(
     [Required][Range(0.01, double.MaxValue)] decimal Price);
}
