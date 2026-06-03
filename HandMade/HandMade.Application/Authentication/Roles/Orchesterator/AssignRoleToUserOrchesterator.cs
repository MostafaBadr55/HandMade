using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Authentication.Roles.Orchesterator
{
    public record AssignRoleToUserOrchesterator(string username, AssignedRole role) : IRequest<RequestResult<AssignedRole>>;

    public class AssignRoleToUserOrchesteratorHandler(IAuthServices authServices) : IRequestHandler<AssignRoleToUserOrchesterator, RequestResult<AssignedRole>>
    {
        public async Task<RequestResult<AssignedRole>> Handle(AssignRoleToUserOrchesterator request, CancellationToken cancellationToken)
        {
            //Find by name (is user exists)
            User? user = await authServices.GetUserByUsernameAsync(request.username);
            if (user is null) 
                return RequestResult<AssignedRole>.Faild(ErrorCode.UserNotFound);

            //validate role exists (RoleExists)
            bool roleExists = await authServices.RoleExistsAsync(request.role);
            if (!roleExists) 
                return RequestResult<AssignedRole>.Faild(ErrorCode.NoRolesFound);

            //GetRoleAsyc( userManager )
            IList<string> userRoles = await authServices.GetRolesAsync(user.Id);
            if (userRoles.Contains(request.role.ToString()))
                return RequestResult<AssignedRole>.Faild(ErrorCode.ThisUserAlreadyHasThisRole);

            //Add to roles
            bool added = await authServices.AddToRoleAsync(user, request.role);
            if (!added)
                return RequestResult<AssignedRole>.Faild(ErrorCode.RoleAddingFaild);
            //change user IsSellerBoolean for Seller choosed role
            if(request.role == AssignedRole.Artist)
            {
                user.IsSeller = true;
                user.UpdatedAt = DateTime.Now;
                var updated = await authServices.UpdateUserAsync(user);
                if (!updated) return RequestResult<AssignedRole>.Faild(ErrorCode.FailedToUpdateSellerBool);
            }
            return RequestResult<AssignedRole>.Success(request.role);
        }
    }

}
