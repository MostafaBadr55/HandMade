using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Orders.Requests
{
    public class CancelOrderRequestVM
    {
        [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}
