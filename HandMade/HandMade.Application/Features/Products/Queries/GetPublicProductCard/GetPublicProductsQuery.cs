using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
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

            // Reviews are polymorphic: TargetId alone is meaningless without
            // TargetType, so this is a filtered correlated subquery rather than a
            // navigation.
            var reviews = unitOfWork.GetRepository<Review>().GetAll();

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
                                    AverageRating = reviews
                                                    .Where(r => r.TargetType == ReviewTargetType.Product && r.TargetId == p.Id)
                                                    .Average(r => (double?)r.Rating),
                                    ReviewCount = reviews
                                                    .Count(r => r.TargetType == ReviewTargetType.Product && r.TargetId == p.Id),
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

