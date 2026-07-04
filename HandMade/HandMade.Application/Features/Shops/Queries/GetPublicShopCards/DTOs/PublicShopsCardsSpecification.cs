using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using HandMade.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetPublicShopCards.DTOs
{
    public class PublicShopsCardsSpecification: BaseSpecification<Shop>
    {
        public PublicShopsCardsSpecification(PublicShopCriteria criteria)
        {
            Criteria = s =>
            s.Status == ShopStatus.Active &&
            (criteria.ShopName == null || s.Name.Contains(criteria.ShopName)) &&
            (criteria.CategoryId == null || 
                s.Products.Any(p =>
                    !p.IsDeleted &&
                     p.ApprovalStatus == ProductApprovalStatus.Approved &&
                     p.CategoryId == criteria.CategoryId));

        }
    }
}
