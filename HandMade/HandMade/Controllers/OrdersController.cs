using HandMade.Application.Features.Orders.Commands.CancelOrder;
using HandMade.Application.Features.Orders.Commands.RejectOrderQuote;
using HandMade.Application.Features.Orders.Orchestrators.AcceptQuoteAndPayAction;
using HandMade.Application.Features.Orders.Orchestrators.CheckoutCartAction;
using HandMade.Application.Features.Orders.Orchestrators.ConfirmDeliveryAction;
using HandMade.Application.Features.Orders.Orchestrators.PlaceOrderRequestAction;
using HandMade.Application.Features.Orders.Queries.GetMyOrderDetails;
using HandMade.Application.Features.Orders.Queries.GetMyOrders;
using HandMade.Application.Features.Orders.Queries.GetMyOrders.FilterHelpers;
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
    /// The buyer side of the order negotiation. The artist drives the other half
    /// (quote, mark complete) from its own controller against the same orders.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(AssignedRole.Client))]
    public class OrdersController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetMyOrders([FromQuery] GetMyOrdersRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var criteria = new MyOrdersCriteria
            {
                Status = request.Status,
                ShopId = request.ShopId,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection
            };

            var response = await _mediator.Send(
                new GetMyOrdersQuery(userId, criteria, request.PageNumber, request.PageSize),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to get your orders", HttpContext.Request.Path);

            var paged = response.Data!.ToPagedResponseVM(o => new OrderListItemResponseVM
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                ShopId = o.ShopId,
                ShopName = o.ShopName,
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
        public async Task<ActionResult> GetOrderDetails(Guid orderId, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new GetMyOrderDetailsQuery(userId, orderId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to get the order", HttpContext.Request.Path);

            var d = response.Data!;

            var details = new OrderDetailsResponseVM
            {
                OrderId = d.OrderId,
                OrderNumber = d.OrderNumber,
                Status = d.Status,
                ShopId = d.ShopId,
                ShopName = d.ShopName,
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
                PaymentStatus = d.PaymentStatus,
                EscrowStatus = d.EscrowStatus,
                AttachmentUrls = d.AttachmentUrls,
                CreatedAt = d.CreatedAt
            };

            return Ok(details);
        }

        /// <summary>Request a quote straight from a product page.</summary>
        [HttpPost("requests")]
        public async Task<ActionResult> PlaceOrderRequest(PlaceOrderRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new PlaceOrderRequestOrchestrator(
                    userId,
                    request.ProductId,
                    request.Quantity,
                    request.ShippingAddressId,
                    request.SpecialInstructions,
                    request.AttachmentPaths),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to place the order request", HttpContext.Request.Path);

            return CreatedAtAction(nameof(GetOrderDetails), new { orderId = response.Data }, new { orderId = response.Data });
        }

        /// <summary>Turn every live cart line into its own negotiation.</summary>
        [HttpPost("checkout")]
        public async Task<ActionResult> CheckoutCart(CheckoutCartRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new CheckoutCartOrchestrator(userId, request.ShippingAddressId, request.SpecialInstructions),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to check out your cart", HttpContext.Request.Path);

            return Created(string.Empty, new { orderIds = response.Data });
        }

        [HttpPatch("{orderId:guid}/accept")]
        public async Task<ActionResult> AcceptQuote(Guid orderId, AcceptOrderQuoteRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new AcceptQuoteAndPayOrchestrator(userId, orderId, request.Method), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to accept the quote", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpPatch("{orderId:guid}/reject")]
        public async Task<ActionResult> RejectQuote(Guid orderId, CancelOrderRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new RejectOrderQuoteCommand(userId, orderId, request.Reason), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to reject the quote", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpPatch("{orderId:guid}/cancel")]
        public async Task<ActionResult> CancelOrder(Guid orderId, CancelOrderRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new CancelOrderCommand(userId, orderId, request.Reason), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to cancel the order", HttpContext.Request.Path);

            return NoContent();
        }

        /// <summary>Confirm receipt — this is what releases the escrowed funds.</summary>
        [HttpPatch("{orderId:guid}/confirm-delivery")]
        public async Task<ActionResult> ConfirmDelivery(Guid orderId, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new ConfirmDeliveryOrchestrator(orderId, userId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to confirm the delivery", HttpContext.Request.Path);

            return NoContent();
        }
    }
}
