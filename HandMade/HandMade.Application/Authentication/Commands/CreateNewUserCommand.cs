using HandMade.Application.Authentication.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Authentication.Commands
{
    public record CreateNewUserCommand(
        string userName,
        string email,
        string password,
        string phoneNumber= "",
        string address= ""
        ) : IRequest<RequestResult<CreatedUserDTO>>;

    public class CreateNewUserCommandHandler : IRequestHandler<CreateNewUserCommand, RequestResult<CreatedUserDTO>>
    {

        private readonly IAuthRepository _authRepository;

        public CreateNewUserCommandHandler(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<RequestResult<CreatedUserDTO>> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
        {
            if (request.userName == String.Empty)
                return RequestResult<CreatedUserDTO>.Faild(ErrorCode.InvalidUserName);
            if(request.email == String.Empty)
                return RequestResult<CreatedUserDTO>.Faild(ErrorCode.InvalidEmail);
            if(request.password == String.Empty)
                return RequestResult<CreatedUserDTO>.Faild(ErrorCode.InvalidPassword);

            var user = new User
            {
                UserName = request.userName,
                Email = request.email,
                PhoneNumber = request.phoneNumber,
                Address = request.address,
                IsSeller= false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _authRepository.CreateAsync(user,request.password);
            if (!result.IsSuccess)
            {
                List<string> errors = result.Errors;
                return RequestResult<CreatedUserDTO>.Success(new CreatedUserDTO(errors, false));
            }

            return RequestResult<CreatedUserDTO>.Success(new CreatedUserDTO(user.Id));
        }
    }
    
    
}
