using HandMade.Application.Features.Addresses.Commands.CreateAddress;
using HandMade.Application.Features.Addresses.Commands.DeleteAddress;
using HandMade.Application.Features.Addresses.Commands.SetDefaultAddress;
using HandMade.Application.Features.Addresses.Commands.UpdateAddress;
using HandMade.Application.Features.Addresses.Queries.GetMyAddresses;
using HandMade.Helpers;
using HandMade.ViewModels.Address;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressesController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetMyAddresses(CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new GetMyAddressesQuery(userId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to get your addresses", HttpContext.Request.Path);

            var addresses = response.Data!.Select(a => new AddressResponseVM
            {
                Id = a.Id,
                Label = a.Label,
                DetailedAddress = a.DetailedAddress,
                IsDefault = a.IsDefault
            });

            return Ok(addresses);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAddress(CreateAddressRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(
                new CreateAddressCommand(userId, request.Label, request.DetailedAddress, request.IsDefault),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to create the address", HttpContext.Request.Path);

            return CreatedAtAction(nameof(GetMyAddresses), new { id = response.Data });
        }

        [HttpPut("{addressId:guid}")]
        public async Task<ActionResult> UpdateAddress(Guid addressId, UpdateAddressRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(
                new UpdateAddressCommand(userId, addressId, request.Label, request.DetailedAddress),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to update the address", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpPatch("{addressId:guid}/default")]
        public async Task<ActionResult> SetDefaultAddress(Guid addressId, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new SetDefaultAddressCommand(userId, addressId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to set the default address", HttpContext.Request.Path);

            return NoContent();
        }

        [HttpDelete("{addressId:guid}")]
        public async Task<ActionResult> DeleteAddress(Guid addressId, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _mediator.Send(new DeleteAddressCommand(userId, addressId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to delete the address", HttpContext.Request.Path);

            return NoContent();
        }
    }
}
