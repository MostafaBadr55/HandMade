using HandMade.Application.Features.Orders.Commands.MarkOrderComplete;
using HandMade.Application.Features.Orders.Commands.RejectOrderRequest;
using HandMade.Application.Features.Orders.Commands.SubmitOrderQuote;
using HandMade.Application.Features.Orders.Queries.GetShopOrderDetails;
using HandMade.Application.Features.Orders.Queries.GetShopOrders;
using HandMade.Application.Features.Orders.Queries.GetShopOrders.FilterHelpers;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels.Orders.Requests;
using HandMade.ViewModels.Orders.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandMade.Controllers
{
    /// <summary>
    /// The artist side of the order negotiation. The client drives the other half
    /// (accept + pay, confirm delivery, cancel) from OrdersController against the
    /// same table — this controller only adds the transitions nothing else covers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(AssignedRole.Artist))]
    public class OrderManagementController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetShopOrders([FromQuery] GetShopOrdersRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var criteria = new ShopOrdersCriteria
            {
                Status = request.Status,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection
            };

            var response = await _mediator.Send(
                new GetShopOrdersQuery(userId, criteria, request.PageNumber, request.PageSize),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to get your shop's orders", HttpContext.Request.Path);

            var paged = response.Data!.ToPagedResponseVM(o => new ShopOrderListItemResponseVM
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                BuyerUserId = o.BuyerUserId,
                BuyerUserName = o.BuyerUserName,
                ProductTitle = o.ProductTitleSnapshot,
                ProductImageUrl = o.ProductImageSnapshot,
                Quantity = o.Quantity,
                GrandTotal = o.GrandTotal,
                ExecutionDays = o.ExecutionDays,
                ExpectedDeliveryDate = o.ExpectedDeliveryDate,
                CreatedAt = o.CreatedAt
            });

            return Ok(paged);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult> GetShopOrderDetails(Guid orderId, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new GetShopOrderDetailsQuery(userId, orderId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to get the order", HttpContext.Request.Path);

            var d = response.Data!;

            var details = new ShopOrderDetailsResponseVM
            {
                OrderId = d.OrderId,
                OrderNumber = d.OrderNumber,
                Status = d.Status,
                BuyerUserId = d.BuyerUserId,
                BuyerUserName = d.BuyerUserName,
                ProductId = d.ProductId,
                ProductTitle = d.ProductTitleSnapshot,
                ProductImageUrl = d.ProductImageSnapshot,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPriceSnapshot,
                Subtotal = d.Subtotal,
                ShippingFee = d.ShippingFee,
                TaxTotal = d.TaxTotal,
                GrandTotal = d.GrandTotal,
                ExecutionDays = d.ExecutionDays,
                SpecialInstructions = d.SpecialInstructions,
                ConfirmedAt = d.ConfirmedAt,
                ExpectedDeliveryDate = d.ExpectedDeliveryDate,
                AutoReleaseAt = d.AutoReleaseAt,
                CancellationReason = d.CancellationReason,
                CancelledAt = d.CancelledAt,
                ShippingAddressLabel = d.ShippingAddressLabel,
                ShippingAddressDetails = d.ShippingAddressDetails,
                AttachmentUrls = d.AttachmentUrls,
                CreatedAt = d.CreatedAt
            };

            return Ok(details);
        }

        /// <summary>SellerPending -> BuyerPending: the artist's price + timeline.</summary>
        [HttpPatch("{orderId:guid}/quote")]
        public async Task<ActionResult> SubmitQuote(Guid orderId, SubmitOrderQuoteRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new SubmitOrderQuoteCommand(userId, orderId, request.Price, request.ExecutionDays),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to submit the quote", HttpContext.Request.Path);

            return NoContent();
        }

        /// <summary>SellerPending -> Cancelled: the artist declines the request outright.</summary>
        [HttpPatch("{orderId:guid}/reject")]
        public async Task<ActionResult> RejectRequest(Guid orderId, RejectOrderRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new RejectOrderRequestCommand(userId, orderId, request.Reason),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to reject the order", HttpContext.Request.Path);

            return NoContent();
        }

        /// <summary>InProgress -> CompletedBySeller: starts the auto-release clock.</summary>
        [HttpPatch("{orderId:guid}/complete")]
        public async Task<ActionResult> MarkComplete(Guid orderId, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(new MarkOrderCompleteCommand(userId, orderId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to mark the order complete", HttpContext.Request.Path);

            return NoContent();
        }
    }
}
