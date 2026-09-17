using HandMade.Application.Features.Carts.Commands.AddItemToCart;
using HandMade.Application.Features.Carts.Commands.GetOrCreateCart;
using HandMade.Application.Features.Carts.Queries.ValidateProductPurchasable;
using HandMade.Application.Shared;
using MediatR;

namespace HandMade.Application.Features.Carts.Orchestrators.AddItemToCartAction
{
    /// <summary>
    /// Full "add to cart" use case: confirm the product can actually be ordered,
    /// make sure the caller has an open cart, then put the line in it. Both
    /// sub-steps are reused elsewhere (the purchasable check also guards direct
    /// order requests), which is what makes this an orchestrator rather than one step.
    /// </summary>
    public record AddItemToCartOrchestrator(
        Guid UserId,
        Guid ProductId,
        int Quantity) : IRequest<RequestResult<Guid>>;

    public class AddItemToCartOrchestratorHandler(IMediator _mediator)
        : IRequestHandler<AddItemToCartOrchestrator, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            AddItemToCartOrchestrator request,
            CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
                return RequestResult<Guid>.Failed(ErrorCode.InvalidQuantity);

            var purchasable = await _mediator.Send(
                new ValidateProductPurchasableQuery(request.ProductId), cancellationToken);

            if (!purchasable.IsSuccess)
                return RequestResult<Guid>.Failed(purchasable.ErrorCode);

            if (!purchasable.Data)
                return RequestResult<Guid>.Failed(ErrorCode.ProductNotPurchasable);

            var cart = await _mediator.Send(
                new GetOrCreateCartCommand(request.UserId), cancellationToken);

            if (!cart.IsSuccess)
                return RequestResult<Guid>.Failed(cart.ErrorCode);

            return await _mediator.Send(
                new AddItemToCartCommand(request.UserId, cart.Data, request.ProductId, request.Quantity),
                cancellationToken);
        }
    }
}
