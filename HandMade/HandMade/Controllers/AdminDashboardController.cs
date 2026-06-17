using HandMade.Application.Features.Categories.Commands.CreateCategory;
using HandMade.Application.Features.Categories.Commands.DeleteCategory;
using HandMade.Application.Features.Categories.Commands.UpdateCategory;
using HandMade.Application.Features.Categories.Queries.GetCategoryManagementDashboard;
using HandMade.Application.Features.Products.Commands;
using HandMade.Application.Features.Products.Commands.ApproveProduct;
using HandMade.Application.Features.Products.Commands.RejectProduct;
using HandMade.Application.Features.Products.Queries.GetProductsForAdmin;
using HandMade.Application.Features.Products.Queries.GetProductsForAdmin.DTOs;
using HandMade.Application.Features.Shops.Commands;
using HandMade.Application.Features.Shops.Commands.ApproveShop;
using HandMade.Application.Features.Shops.Commands.RejectShop;
using HandMade.Application.Features.Shops.Queries.GetShops;
using HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers;
using HandMade.Application.Features.SubCategories.Commands;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels;
using HandMade.ViewModels.AdminDashboard.Requests;
using HandMade.ViewModels.AdminDashboard.Responses;
using HandMade.ViewModels.Category;
using HandMade.ViewModels.SubCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{nameof(AssignedRole.Admin)},SuperAdmin")]
    public class AdminDashboardController(IMediator mediator) : ControllerBase
    {
        [HttpGet("shops")]
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
        public async Task<ActionResult> ApproveShop(Guid shopId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new ApproveShopCommand(shopId), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Unable to approve shop.", HttpContext.Request.Path);

            return Ok(new { message = "Shop approved successfully" });
        }

        [HttpPatch("shops/{shopId:guid}/reject")]
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

        [HttpGet("categories")]
        public async Task<ActionResult<PagedResponseVM<CategoryResponseVM>>> GetCategories([FromQuery] GetAllCategoriesRequestVM request,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new GetCategoryManagementDashboardQuery(request.SearchTerm, request.PageNumber, request.PageSize),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Failed to retrieve categories.", HttpContext.Request.Path);

            var response = result.Data!.ToPagedResponseVM(c => new CategoryResponseVM
            {
                Id = c.Id,
                Name = c.CategoryName,
                Description = c.CategoryDescription,
                ImageUrl = c.CategoryImage,
                CreatedAt = c.CreatedAt,
                SubCategories = c.Subcategories.Select(sc => new SubCategoryResponseVM
                {
                    Id = sc.Id,
                    Name = sc.SubcategoryName,
                    CreatedAt = sc.CreatedAt
                }).ToList()
            });

            return Ok(response);
        }

        [HttpPost("categories")]
        public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryRequestVM request,
        CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new CreateCategoryCommand(request.Name, request.Description, request.ImageUrl),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Failed to create category.", HttpContext.Request.Path);

            return CreatedAtAction(nameof(GetCategories), new { }, result.Data);
        }

        [HttpPut("categories/{id:guid}")]
        public async Task<ActionResult> UpdateCategory(Guid id,[FromBody] UpdateCategoryRequestVM request,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new UpdateCategoryCommand(id, request.Name, request.Description, request.ImageUrl),cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Failed to update category.", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpDelete("categories/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new DeleteCategoryCommand(id),cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Failed to delete category.", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpPost("subcategories")]
        public async Task<ActionResult> CreateSubCategory([FromBody] CreateSubCategoryRequestVM request,
        CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new CreateSubcategoryCommand(request.CategoryId, request.Name),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Failed to create sub-category.", HttpContext.Request.Path);

            return Created($"SubCategory with name: {request.Name} have been created successfully", result.Data);
        }

        [HttpPut("subcategories/{id:guid}")]
        public async Task<ActionResult> Update(Guid id,[FromBody] UpdateSubCategoryRequestVM request,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new UpdateSubcategoryCommand(id, request.CategoryId, request.Name),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Failed to update sub-category.", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpDelete("subcategories/{id:guid}")]
        public async Task<IActionResult> DeleteSubCategory(Guid id,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new DeleteSubCategoryCommand(id),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Failed to delete sub-category.", HttpContext.Request.Path);

            return NoContent();
        }

    }
}
        
    

