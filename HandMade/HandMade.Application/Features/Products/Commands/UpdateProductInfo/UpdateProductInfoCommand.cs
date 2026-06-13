using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Commands.UpdateProductInfo
{
    public record UpdateProductinfoCommand(Guid shopId, Guid productId, string Title, Guid categoryId, Guid subCategoryId) : IRequest<RequestResult<bool>>;

    internal class UpdateProductInfoCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<UpdateProductinfoCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(UpdateProductinfoCommand request, CancellationToken cancellationToken)
        {
            var product = _unitOfWork
            .GetRepository<Product>()
            .GetByIdWithTracking(request.productId)
            .FirstOrDefault();

            if (product is null)
                return RequestResult<bool>.Failed(ErrorCode.ProductNotFound);

            
            if (product.ShopId != request.shopId)
                return RequestResult<bool>.Failed(ErrorCode.ProductDoesNotBelongToThisShop);

            product.Title = request.Title;
            product.CategoryId = request.categoryId;
            product.SubCategoryId = request.subCategoryId;
            product.ApprovalStatus = ProductApprovalStatus.Pending;
            product.IsPublished = false;

            int saved = await _unitOfWork.SaveChangesAsync();

            return RequestResult<bool>.Success(true);
        }
    }


}
