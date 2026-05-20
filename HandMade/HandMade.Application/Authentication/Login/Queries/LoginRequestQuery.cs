using HandMade.Application.Authentication.Login.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Authentication.Login.Queries
{
    public record LoginRequestQuery(string username, string password) : IRequest<RequestResult<LoginResponseDTO>>;

    public class LoginRequestQueryHandler(IAuthRepository authRepository, IAuthTokenService tokenService) : IRequestHandler<LoginRequestQuery, RequestResult<LoginResponseDTO>>
    {
        public async Task<RequestResult<LoginResponseDTO>> Handle(LoginRequestQuery request, CancellationToken cancellationToken)
        {
            User? user = await authRepository.GetUserByUsernameAsync(request.username);
            if (user is null)
                return RequestResult<LoginResponseDTO>.Faild(ErrorCode.UsernameOrPasswordIsInvalid);

            if (!user.IsActive)
                return RequestResult<LoginResponseDTO>.Faild(ErrorCode.InActiveAccount);

            bool? succeded = await authRepository.CheckPasswordAsync(user, request.password);
            
            if(succeded == false || succeded is null)
                return RequestResult<LoginResponseDTO>.Faild(ErrorCode.UsernameOrPasswordIsInvalid);

            //Generate token for this user
            IList<string> roles = await authRepository.GetRolesAsync(user.Id);
            string token = await tokenService.GenerateTokenAsync(user.Id, user.UserName, user.Email, roles);

            LoginResponseDTO response = new LoginResponseDTO { Token = token, UserId = user.Id };
            return RequestResult<LoginResponseDTO>.Success(response);
        }
    }

}
