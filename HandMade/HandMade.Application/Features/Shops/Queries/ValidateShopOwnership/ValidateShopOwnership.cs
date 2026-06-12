using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.ValidateShopOwnership
{
    public record ValidateShopOwnershipQuery(Guid UserId, Guid ShopId) : IRequest<RequestResult<bool>>;

    public class ValidateShopOwnershipQueryHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<ValidateShopOwnershipQuery, RequestResult<bool>>
    {

        public async Task<RequestResult<bool>> Handle(
            ValidateShopOwnershipQuery request,
            CancellationToken cancellationToken)
        {
            var exists = _unitOfWork
                .GetRepository<Shop>()
                .GetAll()
                .Any(s => s.Id == request.ShopId && s.OwnerUserId == request.UserId);

            if (!exists)
                return RequestResult<bool>.Failed(ErrorCode.ShopNotFound);

            return RequestResult<bool>.Success(true);
        }
    }
}
