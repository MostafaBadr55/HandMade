using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.ValidateProductOwnership
{
    public record ValidateProductOwnershipQuery(Guid ProductId, Guid RequestingUserId)
    : IRequest<RequestResult<bool>>;

    public class ValidateProductOwnershipQueryHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<ValidateProductOwnershipQuery, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ValidateProductOwnershipQuery request,
            CancellationToken cancellationToken)
        {
            // Project only what we need — no full entity load
            var product = _unitOfWork
                .GetRepository<Product>()
                .Get(p=> p.Id == request.ProductId)
                .Select(p => new { p.Id, p.Shop.OwnerUserId })
                .FirstOrDefault();

            if (product is null)
                return RequestResult<bool>.Failed(ErrorCode.ProductNotFound);

            if (product.OwnerUserId != request.RequestingUserId)
                return RequestResult<bool>.Failed(ErrorCode.ProductAccessDenied);

            return RequestResult<bool>.Success(true);
        }
    }
}
