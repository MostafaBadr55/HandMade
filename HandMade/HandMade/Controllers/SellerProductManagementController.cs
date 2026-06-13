using HandMade.Application.Features.ProductImages.Commands.CreateProductImages.DTOs;
using HandMade.Application.Features.Products.Commands.UpdateProductPrice;
using HandMade.Application.Features.Products.Commands.UpdateProductStatus;
using HandMade.Application.Features.Products.Orchestrators;
using HandMade.Application.Features.Products.Orchestrators.CreateProductAction;
using HandMade.Application.Features.Products.Orchestrators.UpdateProductMainInfoAction;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerProductManagementController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = nameof(AssignedRole.Artist))]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductRequestVM request,CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _mediator.Send(
                new CreateProductOrchestratorCommand(userId,request.ShopId,request.CategoryId,request.SubCategoryId,request.Title,request.Price,
                    request.Images.Select(i => new CreateProductImageDto(
                        i.RelativePath,
                        i.AltText,
                        i.IsPrimary,
                        i.SortOrder)).ToList()),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Product creation failed.", HttpContext.Request.Path);

            return CreatedAtAction(nameof(CreateProduct), new { id = result.Data }, result.Data);
        }

        [HttpPut("{productId:guid}")]
        public async Task<IActionResult> UpdateMainInfo(Guid shopId,Guid productId,[FromBody] UpdateProductMainInfoRequestVM request,CancellationToken cancellationToken)
        {
            var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _mediator.Send(
                new UpdateProductMainInfoOrchestrator(shopId,sellerId,productId,request.Title,request.CategoryId,request.SubCategoryId,request.Images),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem(
                    "Failed to update product information.", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpPatch("{productId:guid}/price")]
        public async Task<IActionResult> UpdatePrice(Guid shopId,Guid productId,[FromBody] UpdateProductPriceRequestVM request,CancellationToken cancellationToken)
        {
            var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _mediator.Send(
                new UpdateProductPriceCommand(shopId, sellerId, productId, request.Price),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem(
                    "Failed to update product price.", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpPatch("{productId:guid}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid shopId,
            Guid productId,
            [FromBody] UpdateProductStatusRequestVM request,
            CancellationToken cancellationToken)
        {
            var sellerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _mediator.Send(
                new UpdateProductStatusCommand(shopId, sellerId, productId, request.Status),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem(
                    "Failed to update product status.", HttpContext.Request.Path);

            return NoContent();
        }
    }
}
