using HandMade.Application.Features.ProductImages.Commands.CreateProductImages.DTOs;
using HandMade.Application.Features.Products.Commands.UpdateProductPrice;
using HandMade.Application.Features.Products.Commands.UpdateProductStatus;
using HandMade.Application.Features.Products.Orchestrators;
using HandMade.Application.Features.Products.Orchestrators.CreateProductAction;
using HandMade.Application.Features.Products.Orchestrators.DeleteProductAction;
using HandMade.Application.Features.Products.Orchestrators.UpdateProductMainInfoAction;
using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard;
using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs;
using HandMade.Application.Shared;
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
    [Authorize(Roles = nameof(AssignedRole.Artist))]
    public class ProductManagementController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetProductsManagementDashboard(ProductsForSellerCriteria criteria, Guid shopId, int pageNumber, int pageSize, CancellationToken ct)
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var products = await _mediator.Send(new GetProductsForSellerDashboardQuery(criteria, requestingUserId, shopId, pageNumber, pageSize, ct));

            if (!products.IsSuccess)
                return products.ErrorCode.ToProblem("Failed to Get products");

            var response = products.Data.ToPagedResponseVM(product => new ProductForSellerDTO() 
            { Id = product.Id,
              Title = product.Title,
              Status = product.Status,
              IsPublished = product.IsPublished,
              Price = product.Price,
              Images = product.Images
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequestVM request,CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _mediator.Send(
                new CreateProductOrchestratorCommand(userId,request.ShopId,request.CategoryId,request.SubCategoryId,request.Title, request.description, request.Price,
                    request.Images.Select(i => new CreateProductImageDto(
                        i.RelativePath,
                        i.AltText,
                        i.IsPrimary,
                        i.SortOrder)).ToList()),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Product creation failed.", HttpContext.Request.Path);

            return Created(string.Empty,new {message= "Product Created Successfully", id = result.Data});
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
        public async Task<IActionResult> UpdateStatus(Guid shopId,Guid productId,[FromBody] UpdateProductStatusRequestVM request,CancellationToken cancellationToken)
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

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid productId,CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _mediator.Send(
                new DeleteProductOrchestrator(productId, userId),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem(
                    result.ErrorCode switch
                    {
                        ErrorCode.ProductNotFound => "No product with the given ID was found.",
                        ErrorCode.ProductAccessDenied => "You do not have permission to delete this product.",
                        _ => "An error occurred while deleting the product."
                    },
                    HttpContext.Request.Path);

            return NoContent();
        }
    }
}
