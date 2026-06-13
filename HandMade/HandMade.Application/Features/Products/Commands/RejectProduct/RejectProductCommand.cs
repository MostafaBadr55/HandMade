using HandMade.Application.Features.Products.Queries.GetProductStatus;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Commands.RejectProduct
{
    public record RejectProductCommand(Guid productId, string rejectionMessage) : IRequest<RequestResult<bool>>;

    public class RejectProductCommandHandler(IUnitOfWork _unitOfWork, IMediator mediator ) : IRequestHandler<RejectProductCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(RejectProductCommand request, CancellationToken cancellationToken)
        {
            //product status is pending
            var requestApprovalStatus = await mediator.Send(new GetProductStatusQuery(request.productId));
            if (!requestApprovalStatus.IsSuccess)
                return RequestResult<bool>.Failed(requestApprovalStatus.ErrorCode);

            if (requestApprovalStatus.Data != ProductApprovalStatus.Pending)
                return RequestResult<bool>.Failed(ErrorCode.productNotPending);

            //Get tracked product and change status into rejected.
            var product = _unitOfWork.GetRepository<Product>()
                                      .GetByIdWithTracking(request.productId)
                                      .FirstOrDefault();

            product.ApprovalStatus = ProductApprovalStatus.Rejected;
            product.RejectionMessage = request.rejectionMessage;
            product.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }


}
