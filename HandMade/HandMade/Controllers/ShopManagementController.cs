using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs;
using HandMade.Application.Features.Shops.Commands.CreateShop;
using HandMade.Application.Features.Shops.Commands.UpdateShopActivityStatus;
using HandMade.Application.Features.Shops.Commands.UpdateShopInfo;
using HandMade.Application.Features.Shops.Queries.GetMyShop.DTOs;
using HandMade.Application.Features.Shops.Queries.GetShopDashboard;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels;
using HandMade.ViewModels.Shop;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(AssignedRole.Artist))]
    public class ShopManagementController(IMediator _mediator) : ControllerBase
    {
        [HttpGet("myShop")]
        public async Task<ActionResult> GetMyShop(CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            RequestResult<MyShopDTO> response = await _mediator.Send(new GetShopDashboardQuery(userId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to get your shop");

            MyShopResponseVM myShop = new()
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                Status = response.Data.Status,
                Description = response.Data.Description,
                OwnerUserName = User.FindFirstValue(ClaimTypes.Name),
                ImageUrl = response.Data.ImageUrl,
                RatingAverage = response.Data.RatingAverage,
                RejectionMessage = response.Data.RejectionMessage,
                ActiveProductDtos = response.Data.ActiveProductDtos,
                InActiveProductDtos = response.Data.InActiveProductDtos
            };

            return Ok(myShop);
        }
        [HttpPost]
        public async Task<ActionResult> CreateShop(CreateShopRequestVM request, CancellationToken cancellationToken)
        {
            Guid requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _mediator.Send(new CreateShopCommand(requestingUserId, request.ShopName, request.Description, request.imagePath),cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to create the shop");

            return CreatedAtAction(nameof(GetMyShop),new {message= "Your Shop Created and Waiting for approval"});

        }

        [HttpPut]
        public async Task<ActionResult> UpdateShopInfo(UpdateShopInfoRequest request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _mediator.Send(new UpdateShopInfoCommand(userId, request.ShopId, request.ShopName, request.ShopDescription, request.ImageRelativePath));

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to update Your Shop's Info");

            return NoContent();
        }

        [HttpPatch("activity")]
        public async Task<ActionResult> UpdateShopActivityStatus(UpdateShopActivityStatusRequestVM request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await _mediator.Send(new UpdateShopActivityStatusCommand(userId, request.ShopId, request.Status));

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to update Your Shop's Status");

            return NoContent();
        }
    }
}
