using HandMade.Application.Features.Shops.Queries.ValidateShopOwnership;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Commands.UpdateProductStatus
{
    public record UpdateProductStatusCommand(Guid ShopId,Guid SellerId,Guid ProductId,ProductStatus NewStatus) : IRequest<RequestResult<bool>>;

    public class UpdateProductStatusCommandHandler(IUnitOfWork _unitOfWork,IMediator _mediator): IRequestHandler<UpdateProductStatusCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            UpdateProductStatusCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Validate shop ownership
            var ownershipResult = await _mediator.Send(
                new ValidateShopOwnershipQuery(request.SellerId, request.ShopId),
                cancellationToken);

            if (!ownershipResult.IsSuccess)
                return RequestResult<bool>.Failed(ownershipResult.ErrorCode);

            // 2. Load product with tracking
            var product = _unitOfWork
                .GetRepository<Product>()
                .GetByIdWithTracking(request.ProductId)
                .FirstOrDefault();

            if (product is null)
                return RequestResult<bool>.Failed(ErrorCode.ProductNotFound);

            // 3. Verify product belongs to this shop
            if (product.ShopId != request.ShopId)
                return RequestResult<bool>.Failed(ErrorCode.ProductDoesNotBelongToThisShop);

            // 4. Guard: only Approved products can toggle status
            if (product.ApprovalStatus != ProductApprovalStatus.Approved)
                return RequestResult<bool>.Failed(ErrorCode.ProductNotApproved);

            // 5. Apply status change
            product.Status = request.NewStatus;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
