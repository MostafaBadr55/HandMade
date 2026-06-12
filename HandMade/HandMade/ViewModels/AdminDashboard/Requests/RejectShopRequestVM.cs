using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.AdminDashboard.Requests
{
    public class RejectShopRequestVM
    {
        public Guid ShopId { get; set; }
        [Required]
        public string RejectionMessage { get; set; }
    }
}
