using HandMade.Application.Features.Addresses.Queries.ValidateAddressOwnership;
using HandMade.Application.Features.Carts.Queries.ValidateProductPurchasable;
using HandMade.Application.Features.Files.Commands.DeleteFile;
using HandMade.Application.Features.Orders.Commands.CreateOrderAttachments;
using HandMade.Application.Features.Orders.Commands.CreateOrderRequest;
using HandMade.Application.Shared;
using MediatR;

namespace HandMade.Application.Features.Orders.Orchestrators.PlaceOrderRequestAction
{
    /// <summary>
    /// "Request a quote" straight from a product page. Validates the product and the
    /// shipping address, creates the order, then attaches the reference images —
    /// cleaning up the uploaded files if that last step fails, so the disk never
    /// accumulates orphans (same pattern as CreateProductOrchestrator).
    /// </summary>
    public record PlaceOrderRequestOrchestrator(
        Guid UserId,
        Guid ProductId,
        int Quantity,
        Guid ShippingAddressId,
        string? SpecialInstructions,
        IReadOnlyList<string> AttachmentPaths) : IRequest<RequestResult<Guid>>;

    public class PlaceOrderRequestOrchestratorHandler(IMediator _mediator)
        : IRequestHandler<PlaceOrderRequestOrchestrator, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            PlaceOrderRequestOrchestrator request,
            CancellationToken cancellationToken)
        {
            var purchasable = await _mediator.Send(
                new ValidateProductPurchasableQuery(request.ProductId), cancellationToken);

            if (!purchasable.IsSuccess)
                return await FailAndCleanUp(request, purchasable.ErrorCode, cancellationToken);

            if (!purchasable.Data)
                return await FailAndCleanUp(request, ErrorCode.ProductNotPurchasable, cancellationToken);

            var addressOwned = await _mediator.Send(
                new ValidateAddressOwnershipQuery(request.UserId, request.ShippingAddressId), cancellationToken);

            if (!addressOwned.IsSuccess)
                return await FailAndCleanUp(request, addressOwned.ErrorCode, cancellationToken);

            if (!addressOwned.Data)
                return await FailAndCleanUp(request, ErrorCode.AddressNotOwnedByUser, cancellationToken);

            var created = await _mediator.Send(
                new CreateOrderRequestCommand(
                    request.UserId,
                    request.ProductId,
                    request.Quantity,
                    request.ShippingAddressId,
                    request.SpecialInstructions),
                cancellationToken);

            if (!created.IsSuccess)
                return await FailAndCleanUp(request, created.ErrorCode, cancellationToken);

            var attachments = await _mediator.Send(
                new CreateOrderAttachmentsCommand(created.Data, request.AttachmentPaths), cancellationToken);

            if (!attachments.IsSuccess)
                return await FailAndCleanUp(request, attachments.ErrorCode, cancellationToken);

            return RequestResult<Guid>.Success(created.Data);
        }

        private async Task<RequestResult<Guid>> FailAndCleanUp(
            PlaceOrderRequestOrchestrator request,
            ErrorCode errorCode,
            CancellationToken cancellationToken)
        {
            foreach (var path in request.AttachmentPaths)
                await _mediator.Send(new DeleteFileCommand(path), cancellationToken);

            return RequestResult<Guid>.Failed(errorCode);
        }
    }
}
