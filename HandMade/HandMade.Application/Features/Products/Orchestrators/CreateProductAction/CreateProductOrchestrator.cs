using HandMade.Application.Features.Categories.Queries.ValidateCategoryExists;
using HandMade.Application.Features.Files.Commands.DeleteFile;
using HandMade.Application.Features.ProductImages.Commands;
using HandMade.Application.Features.ProductImages.Commands.CreateProductImages;
using HandMade.Application.Features.ProductImages.Commands.CreateProductImages.DTOs;
using HandMade.Application.Features.Products.Commands;
using HandMade.Application.Features.Products.Commands.CreateProduct;
using HandMade.Application.Features.Shops.Queries.ValidateShopOwnership;
using HandMade.Application.Features.SubCategories.Queries;
using HandMade.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Orchestrators.CreateProductAction
{
    public record CreateProductOrchestratorCommand(Guid UserId,Guid ShopId,Guid CategoryId,Guid SubCategoryId,string Title,decimal Price,List<CreateProductImageDto> Images) : IRequest<RequestResult<Guid>>;

    public class CreateProductOrchestratorCommandHandler
    : IRequestHandler<CreateProductOrchestratorCommand, RequestResult<Guid>>
    {
        private readonly IMediator _mediator;

        public CreateProductOrchestratorCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResult<Guid>> Handle(CreateProductOrchestratorCommand request,
            CancellationToken cancellationToken)
        {
            // Step 1 — validate shop ownership
            var ownershipResult = await _mediator.Send(
                new ValidateShopOwnershipQuery(request.UserId, request.ShopId),
                cancellationToken);

            if (!ownershipResult.IsSuccess)
                return RequestResult<Guid>.Failed(ownershipResult.ErrorCode);

            // Step 2 — validate category exists
            var categoryResult = await _mediator.Send(
                new ValidateCategoryExistsQuery(request.CategoryId),
                cancellationToken);

            if (!categoryResult.IsSuccess)
                return RequestResult<Guid>.Failed(categoryResult.ErrorCode);

            // Step 3 — validate subcategory exists and belongs to category (if provided)
                var subCategoryResult = await _mediator.Send(new ValidateSubCategoryExistsQuery(request.SubCategoryId, request.CategoryId),cancellationToken);

                if (!subCategoryResult.IsSuccess)
                    return RequestResult<Guid>.Failed(subCategoryResult.ErrorCode);

                // Step 4 — create the product
                var productResult = await _mediator.Send(
                new CreateProductCommand(request.ShopId,request.CategoryId,request.SubCategoryId,request.Title,request.Price),cancellationToken);

            if (!productResult.IsSuccess)
                return RequestResult<Guid>.Failed(productResult.ErrorCode);

            var productId = productResult.Data;

            // Step 5 — create images if provided
            if (request.Images is { Count: > 0 })
            {
                var imagesResult = await _mediator.Send(
                    new CreateProductImagesCommand(productId, request.Images),
                    cancellationToken);

                // Product exists — orphan image cleanup
                if (!imagesResult.IsSuccess)
                {
                    foreach (var image in request.Images)
                    {
                        await _mediator.Send(
                            new DeleteFileCommand(image.RelativePath),
                            cancellationToken);
                    }

                    return RequestResult<Guid>.Failed(imagesResult.ErrorCode);
                }
            }

            return RequestResult<Guid>.Success(productId);
        }
    }
}
