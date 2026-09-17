using HandMade.Application.Features.Carts.Commands.ClearCart;
using HandMade.Application.Features.Carts.Commands.RemoveCartItem;
using HandMade.Application.Features.Carts.Commands.UpdateCartItemQuantity;
using HandMade.Application.Features.Carts.Orchestrators.AddItemToCartAction;
using HandMade.Application.Features.Carts.Queries.GetMyCart;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels.Cart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(AssignedRole.Client))]
    public class CartController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetMyCart(CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new GetMyCartQuery(userId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to get your cart", HttpContext.Request.Path);

            var cart = new CartResponseVM
            {
                CartId = response.Data!.CartId,
                ItemCount = response.Data.ItemCount,
                Subtotal = response.Data.Subtotal,
                Items = response.Data.Items.Select(i => new CartItemResponseVM
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    ShopId = i.ShopId,
                    ShopName = i.ShopName,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    TotalPrice = i.TotalPrice,
                    ExpectedDays = i.ExpectedDays,
                    ImageUrl = i.ImageUrl,
                    IsStillPurchasable = i.IsStillPurchasable
                }).ToList()
            };

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult> AddItem(AddCartItemRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(
                new AddItemToCartOrchestrator(userId, request.ProductId, request.Quantity),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to add the item to your cart", HttpContext.Request.Path);

            return CreatedAtAction(nameof(GetMyCart), new { cartItemId = response.Data });
        }

        [HttpPatch("items/{cartItemId:guid}")]
        public async Task<ActionResult> UpdateItemQuantity(Guid cartItemId, UpdateCartItemQuantityRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(
                new UpdateCartItemQuantityCommand(userId, cartItemId, request.Quantity),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to update the item quantity", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<ActionResult> RemoveItem(Guid cartItemId, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new RemoveCartItemCommand(userId, cartItemId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to remove the item from your cart", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> ClearCart(CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new ClearCartCommand(userId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to clear your cart", HttpContext.Request.Path);

            return NoContent();
        }
    }
}
