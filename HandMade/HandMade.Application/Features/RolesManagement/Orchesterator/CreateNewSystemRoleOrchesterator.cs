using HandMade.Application.CQRS.RolesManagement.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.CQRS.RolesManagement.Orchesterator
{
    public record CreateNewSystemRoleOrchesterator(string roleName, string roleDescribtion): IRequest<RequestResult<CreateRoleDTO>>;

    public class CreateNewSystemRoleOrchesteratorHandler(IRoleServices roleServices) : IRequestHandler<CreateNewSystemRoleOrchesterator, RequestResult<CreateRoleDTO>>
    {
        public async Task<RequestResult<CreateRoleDTO>> Handle(CreateNewSystemRoleOrchesterator request, CancellationToken cancellationToken)
        {
            var roleExists = await roleServices.RoleExistsAsync(request.roleName);
            if (roleExists)
                return RequestResult<CreateRoleDTO>.Failed(ErrorCode.RoleAlreadyExist);

            CreateRoleDTO dto = new CreateRoleDTO(request.roleName, request.roleDescribtion);
            bool created = await roleServices.AddRoleAsync(dto);
            if (!created)
                return RequestResult<CreateRoleDTO>.Failed(ErrorCode.RoleAddingFaild);

            return RequestResult<CreateRoleDTO>.Success(dto);
            
        }
    }

}
