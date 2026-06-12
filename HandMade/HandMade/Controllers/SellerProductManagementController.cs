using HandMade.Application.Features.ProductImages.Commands.CreateProductImages.DTOs;
using HandMade.Application.Features.Products.Orchestrators;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    }
}
