using HandMade.Domain.DomainEnums;
using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Shop
{
    public class UpdateShopActivityStatusRequestVM
    {
        public Guid ShopId { get; set; }
        [AllowedValues([ShopStatus.Active, ShopStatus.Inactive], ErrorMessage = "Only Active and Inactive are allowed")]
        public ShopStatus Status { get; set; }
    }
}
