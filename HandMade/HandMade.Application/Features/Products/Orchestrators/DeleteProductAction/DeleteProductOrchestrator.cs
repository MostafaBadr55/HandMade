using HandMade.Application.Features.ProductImages.Commands.DeleteProductImages;
using HandMade.Application.Features.Products.Commands.DeleteProduct;
using HandMade.Application.Features.Products.Queries.ValidateProductOwnership;
using HandMade.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Orchestrators.DeleteProductAction
{
    public record DeleteProductOrchestrator(Guid ProductId, Guid RequestingUserId)
     : IRequest<RequestResult<bool>>;

    public class DeleteProductOrchestratorHandler
    : IRequestHandler<DeleteProductOrchestrator, RequestResult<bool>>
    {
        private readonly IMediator _mediator;

        public DeleteProductOrchestratorHandler(IMediator mediator)
            => _mediator = mediator;

        public async Task<RequestResult<bool>> Handle(
            DeleteProductOrchestrator request,
            CancellationToken cancellationToken)
        {
            // Step 1 — Validate ownership
            var ownershipResult = await _mediator.Send(
                new ValidateProductOwnershipQuery(request.ProductId, request.RequestingUserId),
                cancellationToken);

            if (!ownershipResult.IsSuccess)
                return RequestResult<bool>.Failed(ownershipResult.ErrorCode);

            // Step 2 — Delete images (storage + DB records)
            var imagesResult = await _mediator.Send(
                new DeleteProductImagesCommand(request.ProductId),
                cancellationToken);

            if (!imagesResult.IsSuccess)
                return RequestResult<bool>.Failed(imagesResult.ErrorCode);

            // Step 3 — Soft-delete the product
            var deleteResult = await _mediator.Send(
                new DeleteProductCommand(request.ProductId),
                cancellationToken);

            if (!deleteResult.IsSuccess)
                return RequestResult<bool>.Failed(deleteResult.ErrorCode);

            return RequestResult<bool>.Success(true);
        }
    }
}
