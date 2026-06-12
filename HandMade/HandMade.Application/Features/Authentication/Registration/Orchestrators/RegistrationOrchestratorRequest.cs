
using HandMade.Application.Features.Addresses.Commands;
using HandMade.Application.CQRS.Authentication.Registration.Commands;
using HandMade.Application.CQRS.Authentication.Registration.DTOs;
using HandMade.Application.CQRS.Authentication.Registration.Queries;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;


namespace HandMade.Application.CQRS.Authentication.Registration.Orchestrators
{
    public record RegistrationOrchestratorRequest(string email, string userName, string password, string label,string detailedAddress,CancellationToken ct, string phone = "") : IRequest<RequestResult<RegistrationResponseDTO>>;

    public class RegistrationOrchestratorHandler(IMediator mediator, IAuthTokenService authTokenService, IUnitOfWork unitOfWork) : IRequestHandler<RegistrationOrchestratorRequest, RequestResult<RegistrationResponseDTO>>
    {

        public async Task<RequestResult<RegistrationResponseDTO>> Handle(RegistrationOrchestratorRequest request, CancellationToken cancellationToken)
        {
            #region validations
            RequestResult<User> mailExists = await mediator.Send(new FindUserByEmailQuery(request.email));
            if (mailExists.IsSuccess)
                return RequestResult<RegistrationResponseDTO>.Failed(ErrorCode.EmailAlreadyExists);

            RequestResult<User?> userNameExists = await mediator.Send(new GetUserByUserNameQuery(request.userName));
            if (userNameExists.IsSuccess)
                return RequestResult<RegistrationResponseDTO>.Failed(ErrorCode.UsernameAlreadyExists);
            #endregion

            RequestResult<CreatedUserDTO> createResult = await mediator.Send(new CreateNewUserCommand(request.userName, request.email, request.password, request.phone));
            //Check all parameters not empty
            if (!createResult.IsSuccess)
               return RequestResult<RegistrationResponseDTO>.Failed(createResult.ErrorCode);
            //check if the user added to the database
            if (!createResult.Data.Created)
                return RequestResult<RegistrationResponseDTO>.Failed(ErrorCode.UserNotCreated);

            Guid createdUserId = createResult.Data.UserId;
            var createdUser = mediator.Send(new GetUserByIdQuery(createdUserId));
            //Address Creation 
            var addAddressResult = await mediator.Send(new CreateAddressForNewUserCommand(createdUserId,request.label, request.detailedAddress));

            int persist = await unitOfWork.SaveChangesAsync();
            if (persist == 0)
                return RequestResult<RegistrationResponseDTO>.Failed(ErrorCode.DefaultAddressNotAdded);

            RequestResult <IList<string>> rolesResult = await mediator.Send(new GetUserRolesQuery(createResult.Data.UserId));
            IList<string> roles = rolesResult.Data;

            //Generate token
            var token = await authTokenService.GenerateTokenAsync(createResult.Data.UserId,request.userName, request.email, createdUser.Result.Data.SecurityStamp, roles);

            return RequestResult<RegistrationResponseDTO>.Success(new RegistrationResponseDTO { Token = token, UserId = createResult.Data.UserId });

        }
    }


}
