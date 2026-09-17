using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.CreateOrderRequest
{
    /// <summary>
    /// Opens a negotiation: creates the order in SellerPending with the product's
    /// list price snapshotted as the starting figure. The artist's quote may revise
    /// both the price and the timeline, which is why ExecutionDays stays null here.
    /// </summary>
    public record CreateOrderRequestCommand(
        Guid UserId,
        Guid ProductId,
        int Quantity,
        Guid ShippingAddressId,
        string? SpecialInstructions) : IRequest<RequestResult<Guid>>;

    public class CreateOrderRequestCommandHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IOrderNumberGenerator _orderNumberGenerator)
        : IRequestHandler<CreateOrderRequestCommand, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            CreateOrderRequestCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
                return RequestResult<Guid>.Failed(ErrorCode.InvalidQuantity);

            var product = await _executor.FirstOrDefaultAsync(
                _unitOfWork.GetRepository<Product>()
                    .GetById(request.ProductId)
                    .Select(p => new
                    {
                        p.ShopId,
                        p.Title,
                        p.Price,
                        PrimaryImage = p.ProductImages
                            .Where(pi => pi.IsPrimary)
                            .Select(pi => pi.Url)
                            .FirstOrDefault()
                    }),
                cancellationToken);

            if (product is null)
                return RequestResult<Guid>.Failed(ErrorCode.ProductNotFound);

            var subtotal = product.Price * request.Quantity;

            var order = new Order
            {
                UserId = request.UserId,
                ShopId = product.ShopId,
                ProductId = request.ProductId,
                ShippingAddressId = request.ShippingAddressId,
                OrderNumber = _orderNumberGenerator.Generate(),
                Status = OrderStatus.SellerPending,
                Quantity = request.Quantity,
                UnitPriceSnapshot = product.Price,
                ProductTitleSnapshot = product.Title,
                ProductImageSnapshot = product.PrimaryImage,
                Subtotal = subtotal,
                TaxTotal = 0,
                SpecialInstructions = request.SpecialInstructions
            };

            order.GrandTotal = order.Subtotal + order.ShippingFee + order.TaxTotal;

            var orderRepo = _unitOfWork.GetRepository<Order>();
            orderRepo.Add(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<Guid>.Success(order.Id);
        }
    }
}
