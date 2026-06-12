using HandMade.Application.Features.Products.Commands;
using HandMade.Application.Features.Products.Queries.GetProductsForAdmin;
using HandMade.Application.Features.Products.Queries.GetProductsForAdmin.DTOs;
using HandMade.Application.Features.Shops.Commands;
using HandMade.Application.Features.Shops.Commands.ApproveShop;
using HandMade.Application.Features.Shops.Commands.RejectShop;
using HandMade.Application.Features.Shops.Queries.GetShops;
using HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels;
using HandMade.ViewModels.AdminDashboard.Requests;
using HandMade.ViewModels.AdminDashboard.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController(IMediator mediator) : ControllerBase
    {
        [HttpGet("shops")]
        [Authorize(Roles = $"{nameof(AssignedRole.Admin)},SuperAdmin")]
        public async Task<ActionResult<PagedResponseVM<DetailedShopResponseVM>>> GetShops(
            [FromQuery] GetShopsRequestVM request,
            CancellationToken cancellationToken)
        {

            var criteria = new ShopQueryCriteria
            {
                OwnerUserId = request.OwnerUserId,
                Status = request.Status,
                MinRating = request.MinRating,
                MaxRating = request.MaxRating,
                Name = request.Name,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection
            };

            var result = await mediator.Send(
                new GetShopsForAdminQuery(criteria, request.PageNumber, request.PageSize),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem(HttpContext.Request.Path);

            var response = new PagedResponseVM<DetailedShopResponseVM>
            {
                Items = result.Data!.Items.Select(s => new DetailedShopResponseVM
                {
                    Id = s.Id,
                    OwnerUserName = s.OwnerUserName,
                    Name = s.Name,
                    ImageUrl = s.ImageUrl,
                    Description = s.Description,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    RatingAverage = s.RatingAverage
                }).ToList(),
                TotalCount = result.Data!.TotalCount,
                PageNumber = result.Data!.PageNumber,
                PageSize = result.Data!.PageSize,
                TotalPages = result.Data!.TotalPages,
                HasNextPage = result.Data!.HasNextPage,
                HasPreviousPage = result.Data!.HasPreviousPage
            };

            return Ok(response);
        }

        [HttpPatch("shops/{shopId:guid}/approve")]
        [Authorize(Roles = $"{nameof(AssignedRole.Admin)},SuperAdmin")]
        public async Task<ActionResult> ApproveShop(Guid shopId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new ApproveShopCommand(shopId), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Unable to approve shop.", HttpContext.Request.Path);

            return Ok(new { message = "Shop approved successfully" });
        }

        [HttpPatch("shops/{shopId:guid}/reject")]
        [Authorize(Roles = $"{nameof(AssignedRole.Admin)},SuperAdmin")]
        public async Task<ActionResult> RejectShop(RejectShopRequestVM request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new RejectShopCommand(request.ShopId, request.RejectionMessage), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Unable to reject the shop", HttpContext.Request.Path);

            return Ok(new { message = "Shop rejected successfully" });
        }

        [HttpGet("products")]
        public async Task<ActionResult<PagedResponseVM<ProductForAdminResponseVM>>> GetProducts
            ([FromQuery] ProductQueryCriteria criteria, CancellationToken cancellationToken, int pageNumber = 1, int pageSize = 20)
        {
            var result = await mediator.Send(new GetProductsForAdminQuery(criteria, pageNumber, pageSize,cancellationToken));

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Unable to get products");

            var response = new PagedResponseVM<ProductForAdminResponseVM>
            {
                Items = result.Data?.Items.Select(p => new ProductForAdminResponseVM
                {
                    Id = p.Id,
                    Title = p.Title,
                    ShopName = p.ShopName,
                    Status = p.Status,
                    Price = p.Price,
                    IsPublished = p.IsPublished,
                    Images = p.Images
                }).ToList(),
                TotalCount = result.Data!.TotalCount,
                PageNumber = result.Data!.PageNumber,
                PageSize = result.Data!.PageSize,
                TotalPages = result.Data!.TotalPages,
                HasNextPage = result.Data!.HasNextPage,
                HasPreviousPage = result.Data!.HasPreviousPage
            };
            return Ok(response);

        }

        [HttpPatch("products/{productId:Guid}/approve")]
        public async Task<ActionResult> ApproveProduct(Guid productId)
        {
            var result = await mediator.Send(new ApproveProductCommand(productId));

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Product Approve failed");

            return Ok(new { message = "Product Approved Successfully" });
        }

        [HttpPatch("products/{productId:Guid}/reject")]
        public async Task<ActionResult> RejectProduct(RejectProductRequestVM request)
        {
            var result = await mediator.Send(new RejectProductCommand(request.ProductId, request.RejectionMessage));

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Product Rejection failed");

            return Ok(new { message = "Product is rejected Successfully" });
        }
    }
}
        
    

