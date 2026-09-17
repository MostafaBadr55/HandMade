using HandMade.Application.Features.Categories.Queries.GetCategoryCards;
using HandMade.Application.Features.HomePage.Queries.DTOs;
using HandMade.Application.Features.Products.Queries.GetPublicProductCard;
using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Features.Shops.Queries.GetPublicShopCards;
using HandMade.Application.Features.Shops.Queries.GetPublicShopCards.DTOs;
using HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers;
using HandMade.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.HomePage.Queries
{
    public record GetHomePageQuery() : IRequest<RequestResult<HomePageDTO>>;

    public class GetHomePageQueryHandler(IMediator mediator)
    : IRequestHandler<GetHomePageQuery, RequestResult<HomePageDTO>>
    {
        public async Task<RequestResult<HomePageDTO>> Handle(
            GetHomePageQuery request,
            CancellationToken cancellationToken)
        {
            var shopCriteria = new PublicShopCriteria
            {
                SortBy = PublicShopSortBy.Rating,
                Direction = SortDirection.Desc
            };

            var productCriteria = new PublicProductsCriteria
            {
                SortBy = PublicProductSortBy.CreatedAt,
                SortDirection = SortDirection.Desc
            };

            // Sequential, not Task.WhenAll: all three sub-queries resolve the same
            // scoped DbContext, and EF Core forbids concurrent operations on one
            // context instance. Running them in parallel throws
            // "A second operation was started on this context instance".
            var categories = await mediator.Send(
                new GetCategoryCardsQuery(),
                cancellationToken);

            if (!categories.IsSuccess)
                return RequestResult<HomePageDTO>.Failed(categories.ErrorCode);

            var shops = await mediator.Send(
                new GetPublicShopsCardsQuery(shopCriteria, pageNumber: 1, pageSize: 6),
                cancellationToken);

            if (!shops.IsSuccess)
                return RequestResult<HomePageDTO>.Failed(shops.ErrorCode);

            var recentProducts = await mediator.Send(
                new GetPublicProductsQuery(productCriteria, pageNumber: 1, pageSize: 4),
                cancellationToken);

            if (!recentProducts.IsSuccess)
                return RequestResult<HomePageDTO>.Failed(recentProducts.ErrorCode);

            return RequestResult<HomePageDTO>.Success(new HomePageDTO
            {
                Categories = categories.Data!.Items,
                TopRatedShops = shops.Data!.Items,
                MostRecentProducts = recentProducts.Data!.Items
            });
        }
    }
}
