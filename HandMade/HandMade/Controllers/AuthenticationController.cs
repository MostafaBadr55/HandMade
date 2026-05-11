using HandMade.Application.Authentication.Orchestrators;
using HandMade.ViewModels;
using HandMade.ViewModels.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ResponseViewModel<RegistrationResponseVM>> Register([FromBody] RegistrationRequestVM request)
        {
            var registrationResult = await mediator.Send(new RegistrationOrchestratorRequest(request.Email, request.Username, request.Password, request.Phone, request.Address));

            if (!registrationResult.IsSuccess)
             return   ResponseViewModel<RegistrationResponseVM>.Faild(registrationResult.ErrorCode, "Invalid user");

            RegistrationResponseVM response = new RegistrationResponseVM
            {
                Token = registrationResult.Data.Token,
                UserId = registrationResult.Data.UserId
            };

            return ResponseViewModel<RegistrationResponseVM>.Success(response);
        }
    }
}
