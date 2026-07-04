using HandMade.Application.Features.Shops.Queries.GetShops.DTOs;
using HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetShops
{
    public record GetShopsForAdminQuery(ShopQueryCriteria Criteria,int PageNumber,int PageSize) : IRequest<RequestResult<PagedResult<ShopDetailsForAdminDTO>>>;

    public class GetShopsForAdminQueryHandler(
     IUnitOfWork _unitOfWork,IQueryableExecutor _executor,IUrlBuilder _urlBuilder,IAccountServices _accountServices)
     : IRequestHandler<GetShopsForAdminQuery, RequestResult<PagedResult<ShopDetailsForAdminDTO>>>
    {
        public async Task<RequestResult<PagedResult<ShopDetailsForAdminDTO>>> Handle(
            GetShopsForAdminQuery request, CancellationToken cancellationToken)
        {
            var spec = new ShopQuerySpecification(request.Criteria);

            var pagedResult = await _unitOfWork
                .GetRepository<Shop>()
                .GetAll()
                .ApplySpecification(spec)
                .Select(s => new ShopDetailsForAdminDTO
                {
                    Id = s.Id,
                    OwnerUserId = s.OwnerUserId,
                    Name = s.Name,
                    Description = s.Description,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    ImageUrl = _urlBuilder.BuildAbsoluteUrl(s.ImageUrl),
                    RatingAverage = s.RatingAverage
                    // OwnerUserName left empty for now
                })
                .ToPagedResultAsync(_executor, request.PageNumber, request.PageSize, cancellationToken);
            
            //Check in case of no shops found don't continue for the username mapping.
            if (pagedResult.Items.Count == 0)
                return RequestResult<PagedResult<ShopDetailsForAdminDTO>>.Success(pagedResult);

            // Single batch lookup — one DB call for all owner IDs on this page
            var ownerIds = pagedResult.Items.Select(s => s.OwnerUserId).Distinct();
            var usernameMap = await _accountServices.GetUsernamesByIdsAsync(ownerIds);

            foreach (var shop in pagedResult.Items)
                shop.OwnerUserName = usernameMap.GetValueOrDefault(shop.OwnerUserId, string.Empty);

            return RequestResult<PagedResult<ShopDetailsForAdminDTO>>.Success(pagedResult);
        }
    }
}
