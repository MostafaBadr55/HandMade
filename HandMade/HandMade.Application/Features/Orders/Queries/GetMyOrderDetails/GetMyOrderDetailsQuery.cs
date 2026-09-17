using HandMade.Application.Features.Orders.Queries.GetMyOrderDetails.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Queries.GetMyOrderDetails
{
    public record GetMyOrderDetailsQuery(Guid UserId, Guid OrderId) : IRequest<RequestResult<MyOrderDetailsDTO>>;

    public class GetMyOrderDetailsQueryHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IUrlBuilder _urlBuilder)
        : IRequestHandler<GetMyOrderDetailsQuery, RequestResult<MyOrderDetailsDTO>>
    {
        public async Task<RequestResult<MyOrderDetailsDTO>> Handle(
            GetMyOrderDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var order = await _executor.FirstOrDefaultAsync(
                _unitOfWork.GetRepository<Order>()
                    .Get(o => o.Id == request.OrderId && o.UserId == request.UserId)
                    .Select(o => new MyOrderDetailsDTO
                    {
                        OrderId = o.Id,
                        OrderNumber = o.OrderNumber,
                        Status = o.Status,
                        ShopId = o.ShopId,
                        ShopName = o.Shop.Name,
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
                        PaymentStatus = o.Payments
                            .OrderByDescending(p => p.CreatedAt)
                            .Select(p => (Domain.DomainEnums.PaymentStatus?)p.Status)
                            .FirstOrDefault(),
                        EscrowStatus = o.Payments
                            .OrderByDescending(p => p.CreatedAt)
                            .Select(p => (Domain.DomainEnums.EscrowStatus?)p.EscrowStatus)
                            .FirstOrDefault(),
                        AttachmentUrls = o.OrderAttachments
                            .OrderBy(a => a.SortOrder)
                            .Select(a => a.Url)
                            .ToList(),
                        CreatedAt = o.CreatedAt
                    }),
                cancellationToken);

            if (order is null)
                return RequestResult<MyOrderDetailsDTO>.Failed(ErrorCode.OrderNotFound);

            order.ExpectedDeliveryDate = order.ConfirmedAt?.AddDays(order.ExecutionDays ?? 1);

            order.ProductImageSnapshot = order.ProductImageSnapshot is null
                ? null
                : _urlBuilder.BuildAbsoluteUrl(order.ProductImageSnapshot);

            order.AttachmentUrls = order.AttachmentUrls
                .Select(_urlBuilder.BuildAbsoluteUrl)
                .ToList();

            return RequestResult<MyOrderDetailsDTO>.Success(order);
        }
    }
}
