using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetProductStatus
{
    public record GetProductStatusQuery(Guid productId) : IRequest<RequestResult<ProductApprovalStatus>>;

    internal class GetProductStatusQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetProductStatusQuery, RequestResult<ProductApprovalStatus>>
    {
        public async Task<RequestResult<ProductApprovalStatus>> Handle(GetProductStatusQuery request, CancellationToken cancellationToken)
        {
            ProductApprovalStatus? productStatus =  _unitOfWork
                .GetRepository<Product>()
                .GetById(request.productId)
                .Select(p => (ProductApprovalStatus?)p.ApprovalStatus)
                .FirstOrDefault();

            if (productStatus is null)
                return RequestResult<ProductApprovalStatus>.Failed(ErrorCode.ProductNotFound);

            return RequestResult<ProductApprovalStatus>.Success(productStatus.Value);


        }
    }

}
