using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Queries.ValidateProductPurchasable
{
    /// <summary>
    /// A product can be ordered only when all three of its independent axes line up
    /// (approved by an admin, switched on by the artist, published) AND its shop is
    /// still active. Reusable guard for both cart-add and direct order requests.
    /// </summary>
    public record ValidateProductPurchasableQuery(Guid ProductId) : IRequest<RequestResult<bool>>;

    public class ValidateProductPurchasableQueryHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<ValidateProductPurchasableQuery, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ValidateProductPurchasableQuery request,
            CancellationToken cancellationToken)
        {
            var purchasable = await _unitOfWork
                .GetRepository<Product>()
                .AnyAsync(
                    p => p.Id == request.ProductId
                         && p.IsPublished
                         && p.ApprovalStatus == ProductApprovalStatus.Approved
                         && p.Status == ProductStatus.Active
                         && p.Shop.Status == ShopStatus.Active,
                    cancellationToken);

            return RequestResult<bool>.Success(purchasable);
        }
    }
}
