using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Commands.AddItemToCart
{
    public record AddItemToCartCommand(
        Guid UserId,
        Guid CartId,
        Guid ProductId,
        int Quantity) : IRequest<RequestResult<Guid>>;

    public class AddItemToCartCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<AddItemToCartCommand, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            AddItemToCartCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
                return RequestResult<Guid>.Failed(ErrorCode.InvalidQuantity);

            var itemRepo = _unitOfWork.GetRepository<CartItem>();

            // Only *live* lines count: converted ones are history and the unique
            // index is filtered to ignore them.
            var existing = await _executor.FirstOrDefaultAsync(
                itemRepo.GetRangeWithTracking(ci =>
                    ci.CartId == request.CartId
                    && ci.ProductId == request.ProductId
                    && !ci.IsConvertedToOrder),
                cancellationToken);

            if (existing is not null)
            {
                existing.Quantity += request.Quantity;
                existing.TotalPrice = existing.UnitPrice * existing.Quantity;

                itemRepo.Update(existing);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return RequestResult<Guid>.Success(existing.Id);
            }

            var product = await _executor.FirstOrDefaultAsync(
                _unitOfWork.GetRepository<Product>()
                    .GetById(request.ProductId)
                    .Select(p => new { p.Title, p.Price }),
                cancellationToken);

            if (product is null)
                return RequestResult<Guid>.Failed(ErrorCode.ProductNotFound);

            var item = new CartItem
            {
                CartId = request.CartId,
                UserId = request.UserId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                ProductName = product.Title,
                UnitPrice = product.Price,
                TotalPrice = product.Price * request.Quantity
            };

            itemRepo.Add(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<Guid>.Success(item.Id);
        }
    }
}
