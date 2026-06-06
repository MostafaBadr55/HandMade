using HandMade.Application.Features.Shops.Commands;
using HandMade.Application.Features.Shops.Queries.GetShops;
using HandMade.Application.Features.Shops.Queries.GetShops.FilterHelpers;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels;
using HandMade.ViewModels.AdminDashboard;
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

        [HttpPatch("{shopId:guid}/approve")]
        [Authorize(Roles = $"{nameof(AssignedRole.Admin)},SuperAdmin")]
        public async Task<IActionResult> ApproveShop(Guid shopId,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new ApproveShopCommand(shopId), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Unable to approve shop.", HttpContext.Request.Path);

            return NoContent();
        }
    }
}
