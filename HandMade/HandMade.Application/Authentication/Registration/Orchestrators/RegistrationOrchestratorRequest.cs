using HandMade.Application.Addresses.Commands;
using HandMade.Application.Authentication.Registration.Commands;
using HandMade.Application.Authentication.Registration.DTOs;
using HandMade.Application.Authentication.Registration.Queries;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;


namespace HandMade.Application.Authentication.Registration.Orchestrators
{
    public record RegistrationOrchestratorRequest(string email, string userName, string password, string label,string detailedAddress, string phone = "") : IRequest<RequestResult<RegistrationResponseDTO>>;

    public class RegistrationOrchestratorHandler(IMediator mediator, IAuthTokenService authTokenService, IUnitOfWork unitOfWork) : IRequestHandler<RegistrationOrchestratorRequest, RequestResult<RegistrationResponseDTO>>
    {

        public async Task<RequestResult<RegistrationResponseDTO>> Handle(RegistrationOrchestratorRequest request, CancellationToken cancellationToken)
        {
            #region validations
            RequestResult<User> mailExists = await mediator.Send(new FindUserByEmailQuery(request.email));
            if (mailExists.IsSuccess)
                return RequestResult<RegistrationResponseDTO>.Faild(ErrorCode.EmailAlreadyExists);

            RequestResult<User?> userNameExists = await mediator.Send(new GetUserByUserNameQuery(request.userName));
            if (userNameExists.IsSuccess)
                return RequestResult<RegistrationResponseDTO>.Faild(ErrorCode.UsernameAlreadyExists);
            #endregion

            RequestResult<CreatedUserDTO> createResult = await mediator.Send(new CreateNewUserCommand(request.userName, request.email, request.password, request.phone));
            //Check all parameters not empty
            if (!createResult.IsSuccess)
                RequestResult<RegistrationResponseDTO>.Faild(createResult.ErrorCode);
            //check if the user added to the database
            if (!createResult.Data.Created)
                return RequestResult<RegistrationResponseDTO>.Faild(ErrorCode.UserNotCreated);

            //Address Creation 
            var addAddressResult = await mediator.Send(new CreateAddressForNewUserCommand(request.label, request.detailedAddress));

            int persist = await unitOfWork.SaveChangesAsync();
            if (persist == 0)
                return RequestResult<RegistrationResponseDTO>.Faild(ErrorCode.DefaultAddressNotAdded);

            RequestResult <IList<string>> rolesResult = await mediator.Send(new GetUserRolesQuery(createResult.Data.UserId));
            IList<string> roles = rolesResult.Data;

            //Generate token
            var token = await authTokenService.GenerateTokenAsync(createResult.Data.UserId,request.userName, request.email, roles);

            return RequestResult<RegistrationResponseDTO>.Success(new RegistrationResponseDTO { Token = token, UserId = createResult.Data.UserId });

        }
    }


}
