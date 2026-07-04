using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetPublicProductCard
{
    public record GetPublicProductsQuery(PublicProductsCriteria criteria, int pageNumber, int pageSize) : IRequest<RequestResult<PagedResult<PublicProductCardDTO>>>;

    internal class GetPublicProductsQueryHandler(IUnitOfWork unitOfWork, IQueryableExecutor executor, IUrlBuilder urlBuilder)
        : IRequestHandler<GetPublicProductsQuery, RequestResult<PagedResult<PublicProductCardDTO>>>
    {
        public async Task<RequestResult<PagedResult<PublicProductCardDTO>>> Handle(GetPublicProductsQuery request, CancellationToken cancellationToken)
        {
            var spec = new PublicProductsCardsSpecifications(request.criteria);

            var query = unitOfWork.GetRepository<Product>()
                                  .GetAll()
                                  .ApplySpecification(spec)
                                  .Select(p => new PublicProductCardDTO
                                  {
                                    ProductId = p.Id,
                                    ShopId = p.ShopId,
                                    ProductName = p.Title,
                                    ShopName = p.Shop.Name,
                                    Price = p.Price,
                                    ExpectedDays = p.ExpectedDays,
                                    AverageRating = p.Reviews.Any()? p.Reviews.Average(r => (double)r.Rating): null,
                                    ReviewCount = p.Reviews.Count(),
                                    RelativePath = p.ProductImages
                                                    .Where(i => i.IsPrimary)
                                                    .Select(i => i.Url)
                                                    .FirstOrDefault(),
                                    AltText = p.ProductImages
                                                .Where(i => i.IsPrimary)
                                                .Select(i => i.AltText)
                                                .FirstOrDefault()
                                   });

            var pagedResult = await query.ToPagedResultAsync(
                executor,
                request.pageNumber,
                request.pageSize,
                cancellationToken);

            foreach (var item in pagedResult.Items)
            {
                if (item.RelativePath is not null)
                    item.RelativePath = urlBuilder.BuildAbsoluteUrl(item.RelativePath);
            }

            return RequestResult<PagedResult<PublicProductCardDTO>>.Success(pagedResult);
        }
    }

}

