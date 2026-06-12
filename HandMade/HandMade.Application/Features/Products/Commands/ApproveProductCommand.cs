using HandMade.Application.Features.Products.Queries.GetProductStatus;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Commands
{
    public record ApproveProductCommand(Guid productId) : IRequest<RequestResult<bool>>;

    internal class ApproveProductCommandHandler(IUnitOfWork _unitOfWork, IMediator mediator) : IRequestHandler<ApproveProductCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(ApproveProductCommand request, CancellationToken cancellationToken)
        {
            //product status is pending
            var requestApprovalStatus = await mediator.Send(new GetProductStatusQuery(request.productId));
            if (!requestApprovalStatus.IsSuccess)
                return RequestResult<bool>.Failed(requestApprovalStatus.ErrorCode);

            if (requestApprovalStatus.Data != ProductApprovalStatus.Pending)
                return RequestResult<bool>.Failed(ErrorCode.productNotPending);

            //Get tracked product and change status into approved.
            var product = _unitOfWork.GetRepository<Product>()
                                      .GetByIdWithTracking(request.productId)
                                      .FirstOrDefault();

            product.ApprovalStatus = ProductApprovalStatus.Approved;
            product.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }

}
