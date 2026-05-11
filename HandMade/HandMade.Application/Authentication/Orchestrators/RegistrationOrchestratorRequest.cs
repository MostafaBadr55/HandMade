using HandMade.Application.Authentication.Commands;
using HandMade.Application.Authentication.DTOs;
using HandMade.Application.Authentication.Queries;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;


namespace HandMade.Application.Authentication.Orchestrators
{
    public record RegistrationOrchestratorRequest(string email, string userName, string password, string phone="", string address="") : IRequest<RequestResult<RegistrationResponseDTO>>;

    public class RegistrationOrchestratorHandler(IMediator mediator, IAuthTokenService authTokenService) : IRequestHandler<RegistrationOrchestratorRequest, RequestResult<RegistrationResponseDTO>>
    {

        public async Task<RequestResult<RegistrationResponseDTO>> Handle(RegistrationOrchestratorRequest request, CancellationToken cancellationToken)
        {
            RequestResult<User> mailExists = await mediator.Send(new FindUserByEmailQuery(request.email));
            if (mailExists.IsSuccess)
                return RequestResult<RegistrationResponseDTO>.Faild(ErrorCode.EmailAlreadyExists);

            RequestResult<User?> userNameExists = await mediator.Send(new GetUserByUserNameQuery(request.userName));
            if (userNameExists.IsSuccess)
                return RequestResult<RegistrationResponseDTO>.Faild(ErrorCode.UsernameAlreadyExists);

            RequestResult<CreatedUserDTO> createResult = await mediator.Send(new CreateNewUserCommand(request.userName, request.email, request.password, request.phone, request.address));
            //Check all parameters not empty
            if (!createResult.IsSuccess)
                RequestResult<RegistrationResponseDTO>.Faild(createResult.ErrorCode);
            //check if the user added to the database
            if (!createResult.Data.Created)
                return RequestResult<RegistrationResponseDTO>.Faild(ErrorCode.UserNotCreated);

            RequestResult<IList<string>> rolesResult = await mediator.Send(new GetUserRolesQuery(createResult.Data.UserId));
            IList<string> roles = rolesResult.Data;

            //Generate token
            var token = await authTokenService.GenerateTokenAsync(createResult.Data.UserId,request.userName, request.email, roles);

            return RequestResult<RegistrationResponseDTO>.Success(new RegistrationResponseDTO { Token = token, UserId = createResult.Data.UserId });

        }
    }


}
