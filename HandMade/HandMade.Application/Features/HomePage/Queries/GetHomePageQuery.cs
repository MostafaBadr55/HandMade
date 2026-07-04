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

            var categoriesTask = mediator.Send(
                new GetCategoryCardsQuery(),
                cancellationToken);

            var shopsTask = mediator.Send(
                new GetPublicShopsCardsQuery(shopCriteria, pageNumber: 1, pageSize: 6),
                cancellationToken);

            var productsTask = mediator.Send(
                new GetPublicProductsQuery(productCriteria, pageNumber: 1, pageSize: 4),
                cancellationToken);

            await Task.WhenAll(categoriesTask, shopsTask, productsTask);

            return RequestResult<HomePageDTO>.Success(new HomePageDTO
            {
                Categories = categoriesTask.Result.Data!.Items,
                TopRatedShops = shopsTask.Result.Data!.Items,
                MostRecentProducts = productsTask.Result.Data!.Items
            });
        }
    }
}
