using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.AdminDashboard
{
    public class DetailedShopResponseVM
    {
        public Guid Id { get; set; }
        public string OwnerUserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ShopStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal RatingAverage { get; set; }
    }
}
