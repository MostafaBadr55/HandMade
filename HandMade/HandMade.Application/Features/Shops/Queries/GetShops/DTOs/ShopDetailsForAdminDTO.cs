using HandMade.Domain.DomainEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetShops.DTOs
{
    public class ShopDetailsForAdminDTO
    {
        public Guid Id { get; set; }
        public string OwnerUserName { get; set; }
        public Guid OwnerUserId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public ShopStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal RatingAverage { get; set; }
    }
}
