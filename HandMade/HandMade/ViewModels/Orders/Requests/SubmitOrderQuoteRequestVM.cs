using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Orders.Requests
{
    public class SubmitOrderQuoteRequestVM
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(1, 365, ErrorMessage = "ExecutionDays must be between 1 and 365.")]
        public int ExecutionDays { get; set; }
    }
}
