using HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.AdminDashboard
{
    public class GetShopsRequestVM
    {
        public Guid? OwnerUserId { get; set; }
        public ShopStatus? Status { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? MaxRating { get; set; }
        public string? Name { get; set; }
        public ShopSortBy SortBy { get; set; } = ShopSortBy.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
