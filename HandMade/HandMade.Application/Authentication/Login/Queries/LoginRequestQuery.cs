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

    public class LoginRequestQueryHandler(IAuthServices authService, IAuthTokenService tokenService) : IRequestHandler<LoginRequestQuery, RequestResult<LoginResponseDTO>>
    {
        public async Task<RequestResult<LoginResponseDTO>> Handle(LoginRequestQuery request, CancellationToken cancellationToken)
        {
            User? user = await authService.GetUserByUsernameAsync(request.username);
            if (user is null)
                return RequestResult<LoginResponseDTO>.Faild(ErrorCode.InvalidUsernameOrPassword);

            if (!user.IsActive)
                return RequestResult<LoginResponseDTO>.Faild(ErrorCode.InActiveAccount);

            bool? succeded = await authService.CheckPasswordAsync(user, request.password);
            
            if(succeded == false || succeded is null)
                return RequestResult<LoginResponseDTO>.Faild(ErrorCode.InvalidUsernameOrPassword);

            //Generate token for this user
            IList<string> roles = await authService.GetRolesAsync(user.Id);
            string token = await tokenService.GenerateTokenAsync(user.Id, user.UserName, user.Email,user.SecurityStamp, roles);

            LoginResponseDTO response = new LoginResponseDTO { Token = token, UserId = user.Id };
            return RequestResult<LoginResponseDTO>.Success(response);
        }
    }

}
