using HandMade.Application.Authentication.Login.DTOs;
using HandMade.Application.Authentication.Login.Queries;
using HandMade.Application.Authentication.Registration.Orchestrators;
using HandMade.Application.Authentication.Roles.Orchesterator;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels;
using HandMade.ViewModels.Authentication;
using HandMade.ViewModels.Authentication.Login;
using HandMade.ViewModels.Authentication.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<ResponseViewModel<RegistrationResponseVM>>> Register(RegistrationRequestVM request)
        {
            var registrationResult = await mediator.Send(new RegistrationOrchestratorRequest(
                request.Email, request.Username, request.Password,
                request.AddressLable, request.Address, request.Phone));

            if (!registrationResult.IsSuccess)
                return registrationResult.ErrorCode.ToProblem("Invalid user");

            var response = ResponseViewModel<RegistrationResponseVM>.Success(new RegistrationResponseVM
            {
                Token = registrationResult.Data.Token,
                UserId = registrationResult.Data.UserId
            });

            return response;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseViewModel<LoginResponseVM>>> Login(LoginRequestVM request)
        {
            RequestResult<LoginResponseDTO> result = await mediator.Send(
                new LoginRequestQuery(request.Username, request.password));

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Invalid username or password");

            var response = ResponseViewModel<LoginResponseVM>.Success(new LoginResponseVM
            {
                Token = result.Data.Token,
                UserId = result.Data.UserId
            });

            return response;
        }

        [HttpPost("users/select-role")]
        [Authorize]
        public async Task<ActionResult<ResponseViewModel<SelectRoleRequestVM>>> AssignRole(SelectRoleRequestVM request)
        {
            RequestResult<AssignedRole> result = await mediator.Send(
                new AssignRoleToUserOrchesterator(request.Username, request.SelectedRole));

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Role not assigned");

            var response = ResponseViewModel<SelectRoleResponseVM>.Success(new SelectRoleResponseVM
            {
                Role = request.SelectedRole,
                Username = request.Username
            });

            return Ok(response);
        }

    }
}
