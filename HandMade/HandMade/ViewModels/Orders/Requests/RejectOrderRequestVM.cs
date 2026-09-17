using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Orders.Requests
{
    public class RejectOrderRequestVM
    {
        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
