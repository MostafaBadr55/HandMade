using HandMade.Application.Features.Orders.Queries.GetShopOrderDetails.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Queries.GetShopOrderDetails
{
    public record GetShopOrderDetailsQuery(Guid UserId, Guid OrderId) : IRequest<RequestResult<ShopOrderDetailsDTO>>;

    public class GetShopOrderDetailsQueryHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IUrlBuilder _urlBuilder,
        IAccountServices _accountServices)
        : IRequestHandler<GetShopOrderDetailsQuery, RequestResult<ShopOrderDetailsDTO>>
    {
        public async Task<RequestResult<ShopOrderDetailsDTO>> Handle(
            GetShopOrderDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var order = await _executor.FirstOrDefaultAsync(
                _unitOfWork.GetRepository<Order>()
                    .Get(o => o.Id == request.OrderId && o.Shop.OwnerUserId == request.UserId)
                    .Select(o => new ShopOrderDetailsDTO
                    {
                        OrderId = o.Id,
                        OrderNumber = o.OrderNumber,
                        Status = o.Status,
                        BuyerUserId = o.UserId,
                        ProductId = o.ProductId,
                        ProductTitleSnapshot = o.ProductTitleSnapshot,
                        ProductImageSnapshot = o.ProductImageSnapshot,
                        Quantity = o.Quantity,
                        UnitPriceSnapshot = o.UnitPriceSnapshot,
                        Subtotal = o.Subtotal,
                        ShippingFee = o.ShippingFee,
                        TaxTotal = o.TaxTotal,
                        GrandTotal = o.GrandTotal,
                        ExecutionDays = o.ExecutionDays,
                        SpecialInstructions = o.SpecialInstructions,
                        ConfirmedAt = o.ConfirmedAt,
                        AutoReleaseAt = o.AutoReleaseAt,
                        CancellationReason = o.CancellationReason,
                        CancelledAt = o.CancelledAt,
                        ShippingAddressLabel = o.ShippingAddress.Label,
                        ShippingAddressDetails = o.ShippingAddress.DetailedAddress,
                        AttachmentUrls = o.OrderAttachments
                            .OrderBy(a => a.SortOrder)
                            .Select(a => a.Url)
                            .ToList(),
                        CreatedAt = o.CreatedAt
                    }),
                cancellationToken);

            if (order is null)
                return RequestResult<ShopOrderDetailsDTO>.Failed(ErrorCode.OrderNotFound);

            order.ExpectedDeliveryDate = order.ConfirmedAt?.AddDays(order.ExecutionDays ?? 1);

            order.ProductImageSnapshot = order.ProductImageSnapshot is null
                ? null
                : _urlBuilder.BuildAbsoluteUrl(order.ProductImageSnapshot);

            order.AttachmentUrls = order.AttachmentUrls
                .Select(_urlBuilder.BuildAbsoluteUrl)
                .ToList();

            var usernameMap = await _accountServices.GetUsernamesByIdsAsync([order.BuyerUserId]);
            order.BuyerUserName = usernameMap.GetValueOrDefault(order.BuyerUserId, string.Empty);

            return RequestResult<ShopOrderDetailsDTO>.Success(order);
        }
    }
}
