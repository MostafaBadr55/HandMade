using HandMade.Application.Features.Categories.Queries.ValidateCategoryExists;
using HandMade.Application.Features.ProductImages.Commands.UpdateProductImages;
using HandMade.Application.Features.ProductImages.Commands.UpdateProductImages.DTOs;
using HandMade.Application.Features.Products.Commands.UpdateProductInfo;
using HandMade.Application.Features.Shops.Queries.ValidateShopOwnership;
using HandMade.Application.Features.SubCategories.Queries;
using HandMade.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Orchestrators.UpdateProductMainInfoAction
{
    public record UpdateProductMainInfoOrchestrator(Guid ShopId,Guid SellerId,Guid ProductId,string Title,Guid CategoryId,Guid SubCategoryId,List<UpdateProductImageDTO> Images) : IRequest<RequestResult<bool>>;

    internal class UpdateProductMainInfoOrchestratorHandler(IMediator _mediator) : IRequestHandler<UpdateProductMainInfoOrchestrator, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(UpdateProductMainInfoOrchestrator request, CancellationToken cancellationToken)
        {
            // 1. Validate shop ownership
            var ownershipResult = await _mediator.Send(
                new ValidateShopOwnershipQuery(request.SellerId, request.ShopId),
                cancellationToken);

            if (!ownershipResult.IsSuccess)
                return RequestResult<bool>.Failed(ownershipResult.ErrorCode);

            // 2. Validate category exists
            var categoryResult = await _mediator.Send(
                new ValidateCategoryExistsQuery(request.CategoryId),
                cancellationToken);

            if (!categoryResult.IsSuccess)
                return RequestResult<bool>.Failed(categoryResult.ErrorCode);

            // 3. Validate subcategory belongs to category
            var subCategoryResult = await _mediator.Send(
                new ValidateSubCategoryExistsQuery(request.SubCategoryId, request.CategoryId),
                cancellationToken);

            if (!subCategoryResult.IsSuccess)
                return RequestResult<bool>.Failed(subCategoryResult.ErrorCode);

            // 4. Commit product info changes (SaveChangesAsync owned by this step)
            var updateInfoResult = await _mediator.Send(
                new UpdateProductinfoCommand(
                    request.ShopId,
                    request.ProductId,
                    request.Title,
                    request.CategoryId,
                    request.SubCategoryId),
                cancellationToken);

            if (!updateInfoResult.IsSuccess)
                return RequestResult<bool>.Failed(updateInfoResult.ErrorCode);

            // 5. Commit image changes (SaveChangesAsync owned by this step)
            var updateImagesResult = await _mediator.Send(
                new UpdateProductImagesCommand(request.ProductId, request.Images),
                cancellationToken);

            if (!updateImagesResult.IsSuccess)
                return RequestResult<bool>.Failed(updateImagesResult.ErrorCode);

            return RequestResult<bool>.Success(true);


        }
    }
}
