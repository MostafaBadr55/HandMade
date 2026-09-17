using HandMade.Application.Features.Carts.Queries.GetMyCart.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Queries.GetMyCart
{
    public record GetMyCartQuery(Guid UserId) : IRequest<RequestResult<CartDTO>>;

    public class GetMyCartQueryHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IUrlBuilder _urlBuilder)
        : IRequestHandler<GetMyCartQuery, RequestResult<CartDTO>>
    {
        public async Task<RequestResult<CartDTO>> Handle(
            GetMyCartQuery request,
            CancellationToken cancellationToken)
        {
            var cart = await _executor.FirstOrDefaultAsync(
                _unitOfWork.GetRepository<Cart>()
                    .Get(c => c.UserId == request.UserId)
                    .Select(c => new { c.Id, c.CheckedOutAt }),
                cancellationToken);

            if (cart is null)
                return RequestResult<CartDTO>.Failed(ErrorCode.CartNotFound);

            var items = await _executor.ToListAsync(
                _unitOfWork.GetRepository<CartItem>()
                    .Get(ci => ci.CartId == cart.Id && !ci.IsConvertedToOrder)
                    .OrderBy(ci => ci.CreatedAt)
                    .Select(ci => new CartItemDTO
                    {
                        CartItemId = ci.Id,
                        ProductId = ci.ProductId,
                        ShopId = ci.Product.ShopId,
                        ShopName = ci.Product.Shop.Name,
                        ProductName = ci.ProductName,
                        UnitPrice = ci.UnitPrice,
                        Quantity = ci.Quantity,
                        TotalPrice = ci.TotalPrice,
                        ExpectedDays = ci.Product.ExpectedDays,
                        ImageUrl = ci.Product.ProductImages
                            .Where(pi => pi.IsPrimary)
                            .Select(pi => pi.Url)
                            .FirstOrDefault(),
                        IsStillPurchasable =
                            ci.Product.IsPublished
                            && ci.Product.ApprovalStatus == ProductApprovalStatus.Approved
                            && ci.Product.Status == ProductStatus.Active
                            && ci.Product.Shop.Status == ShopStatus.Active
                    }),
                cancellationToken);

            foreach (var item in items)
                item.ImageUrl = item.ImageUrl is null ? null : _urlBuilder.BuildAbsoluteUrl(item.ImageUrl);

            var dto = new CartDTO
            {
                CartId = cart.Id,
                CheckedOutAt = cart.CheckedOutAt,
                Items = items,
                ItemCount = items.Count,
                Subtotal = items.Sum(i => i.TotalPrice)
            };

            return RequestResult<CartDTO>.Success(dto);
        }
    }
}
