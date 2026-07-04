using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Features.Shops.Queries.GetPublicShopCards.DTOs;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetPublicShopCards
{
    public record GetPublicShopsCardsQuery(PublicShopCriteria criteria, int pageNumber, int pageSize) : IRequest<RequestResult<PagedResult<PublicShopCardDTO>>>;

    public class GetPublicShopsCardsQueryHandler(IUnitOfWork unitOfWork, IQueryableExecutor executor, IUrlBuilder urlBuilder)
        : IRequestHandler<GetPublicShopsCardsQuery, RequestResult<PagedResult<PublicShopCardDTO>>>
    {
        public async Task<RequestResult<PagedResult<PublicShopCardDTO>>> Handle(GetPublicShopsCardsQuery request, CancellationToken cancellationToken)
        {
            var spec = new PublicShopsCardsSpecification(request.criteria);

            var query = unitOfWork.GetRepository<Shop>()
                                  .GetAll()
                                  .ApplySpecification(spec)
                                  .Select(s => new PublicShopCardDTO
                                  {
                                      ShopId = s.Id,
                                      ShopName = s.Name,
                                      Description = s.Description,
                                      MainImage = s.ImageUrl,
                                      Rating = s.Reviews.Any() ? s.Reviews.Average(r => (double)r.Rating) : 0
                                  });

            var pagedResult = await query.ToPagedResultAsync(
                executor,
                request.pageNumber,
                request.pageSize,
                cancellationToken);

            foreach (var item in pagedResult.Items)
            {
                if (item.MainImage is not null)
                    item.MainImage = urlBuilder.BuildAbsoluteUrl(item.MainImage);
            }

            return RequestResult<PagedResult<PublicShopCardDTO>>.Success(pagedResult);
        }
    }


}
