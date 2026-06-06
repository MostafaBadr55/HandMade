using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.CQRS.RolesManagement.Orchesterator
{
    public record AssignRoleToUserOrchesterator(string username, AssignedRole role) : IRequest<RequestResult<AssignedRole>>;

    public class AssignRoleToUserOrchesteratorHandler(IAccountServices authServices) : IRequestHandler<AssignRoleToUserOrchesterator, RequestResult<AssignedRole>>
    {
        public async Task<RequestResult<AssignedRole>> Handle(AssignRoleToUserOrchesterator request, CancellationToken cancellationToken)
        {
            //Find by name (is user exists)
            User? user = await authServices.GetUserByUsernameAsync(request.username);
            if (user is null) 
                return RequestResult<AssignedRole>.Failed(ErrorCode.UserNotFound);

            //validate role exists (RoleExists)
            bool roleExists = await authServices.RoleExistsAsync(request.role);
            if (!roleExists) 
                return RequestResult<AssignedRole>.Failed(ErrorCode.NoRolesFound);

            //GetRoleAsyc( userManager )
            IList<string> userRoles = await authServices.GetRolesAsync(user.Id);
            if (userRoles.Contains(request.role.ToString()))
                return RequestResult<AssignedRole>.Failed(ErrorCode.ThisUserAlreadyHasThisRole);

            //Add to roles
            bool added = await authServices.AddToRoleAsync(user.Id.ToString(), request.role);
            if (!added)
                return RequestResult<AssignedRole>.Failed(ErrorCode.RoleAddingFaild);
            //Adding both Client and Artist in case of choosing to be an Artist
            if (request.role == AssignedRole.Artist)
            {
                bool clientRoleExists = await authServices.RoleExistsAsync(AssignedRole.Client);
                if (clientRoleExists && !userRoles.Contains(AssignedRole.Client.ToString()))
                {
                    bool clientAdded = await authServices.AddToRoleAsync(user.Id.ToString(), AssignedRole.Client);
                    if (!clientAdded)
                        return RequestResult<AssignedRole>.Failed(ErrorCode.RoleAddingFaild);
                }

                user.IsSeller = true;
                user.UpdatedAt = DateTime.Now;
                bool updated = await authServices.UpdateUserAsync(user);
                if (!updated)
                    return RequestResult<AssignedRole>.Failed(ErrorCode.FailedToUpdateSellerBool);
            }

            return RequestResult<AssignedRole>.Success(request.role);
        }
    }

}
