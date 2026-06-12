namespace HandMade.ViewModels.AdminDashboard.Requests
{
    public class RejectProductRequestVM
    {
        public Guid ProductId { get; set; }
        public string RejectionMessage { get; set; }
    }
}
