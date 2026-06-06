using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetShopStatus
{
    public record GetShopStatusQuery(Guid ShopId) : IRequest<RequestResult<ShopStatus>>;

    internal sealed class GetShopStatusQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetShopStatusQuery, RequestResult<ShopStatus>>
    {
        public async Task<RequestResult<ShopStatus>> Handle(
            GetShopStatusQuery request,
            CancellationToken cancellationToken)
        {
            var status = unitOfWork
                .GetRepository<Shop>()
                .GetById(request.ShopId)
                .Select(s => (ShopStatus?)s.Status)
                .FirstOrDefault();

            if (status is null)
                return RequestResult<ShopStatus>.Failed(ErrorCode.ShopNotFound);

            return RequestResult<ShopStatus>.Success(status.Value);
        }
    }
}
