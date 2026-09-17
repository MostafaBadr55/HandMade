using HandMade.Application.Features.Addresses.Queries.ValidateAddressOwnership;
using HandMade.Application.Features.Carts.Commands.MarkCartCheckedOut;
using HandMade.Application.Features.Carts.Queries.GetMyCart;
using HandMade.Application.Features.Carts.Queries.ValidateProductPurchasable;
using HandMade.Application.Features.Orders.Commands.CreateOrderRequest;
using HandMade.Application.Features.Orders.Commands.MarkCartItemConverted;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using MediatR;

namespace HandMade.Application.Features.Orders.Orchestrators.CheckoutCartAction
{
    /// <summary>
    /// Turns a cart into negotiations: every live line becomes its own Order in
    /// SellerPending, because Order is single-product and each artist quotes
    /// separately. All-or-nothing — a cart that is half converted would leave the
    /// client with orders they never agreed to, so the fan-out runs in one
    /// transaction.
    /// </summary>
    public record CheckoutCartOrchestrator(
        Guid UserId,
        Guid ShippingAddressId,
        string? SpecialInstructions) : IRequest<RequestResult<List<Guid>>>;

    public class CheckoutCartOrchestratorHandler(IMediator _mediator, IUnitOfWork _unitOfWork)
        : IRequestHandler<CheckoutCartOrchestrator, RequestResult<List<Guid>>>
    {
        public async Task<RequestResult<List<Guid>>> Handle(
            CheckoutCartOrchestrator request,
            CancellationToken cancellationToken)
        {
            var cart = await _mediator.Send(new GetMyCartQuery(request.UserId), cancellationToken);

            if (!cart.IsSuccess)
                return RequestResult<List<Guid>>.Failed(cart.ErrorCode);

            if (cart.Data!.Items.Count == 0)
                return RequestResult<List<Guid>>.Failed(ErrorCode.CartIsEmpty);

            var addressOwned = await _mediator.Send(
                new ValidateAddressOwnershipQuery(request.UserId, request.ShippingAddressId), cancellationToken);

            if (!addressOwned.IsSuccess)
                return RequestResult<List<Guid>>.Failed(addressOwned.ErrorCode);

            if (!addressOwned.Data)
                return RequestResult<List<Guid>>.Failed(ErrorCode.AddressNotOwnedByUser);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var orderIds = new List<Guid>(cart.Data.Items.Count);

                foreach (var item in cart.Data.Items)
                {
                    var purchasable = await _mediator.Send(
                        new ValidateProductPurchasableQuery(item.ProductId), cancellationToken);

                    if (!purchasable.IsSuccess || !purchasable.Data)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return RequestResult<List<Guid>>.Failed(ErrorCode.ProductNotPurchasable);
                    }

                    var created = await _mediator.Send(
                        new CreateOrderRequestCommand(
                            request.UserId,
                            item.ProductId,
                            item.Quantity,
                            request.ShippingAddressId,
                            request.SpecialInstructions),
                        cancellationToken);

                    if (!created.IsSuccess)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return RequestResult<List<Guid>>.Failed(created.ErrorCode);
                    }

                    var converted = await _mediator.Send(
                        new MarkCartItemConvertedCommand(item.CartItemId, created.Data), cancellationToken);

                    if (!converted.IsSuccess)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return RequestResult<List<Guid>>.Failed(converted.ErrorCode);
                    }

                    orderIds.Add(created.Data);
                }

                var checkedOut = await _mediator.Send(
                    new MarkCartCheckedOutCommand(cart.Data.CartId), cancellationToken);

                if (!checkedOut.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return RequestResult<List<Guid>>.Failed(checkedOut.ErrorCode);
                }

                await transaction.CommitAsync(cancellationToken);

                return RequestResult<List<Guid>>.Success(orderIds);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
