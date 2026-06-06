using HandMade.Application.CQRS.RolesManagement.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.CQRS.RolesManagement.Queries
{
    public record GetSytemRolesQuery() : IRequest<RequestResult<List<SystemRoleDTO>>>;

    public class GetSystemRolesQueryHandler(IRoleServices roleServices) : IRequestHandler<GetSytemRolesQuery, RequestResult<List<SystemRoleDTO>>>
    {
        public async Task<RequestResult<List<SystemRoleDTO>>> Handle(GetSytemRolesQuery request, CancellationToken cancellationToken)
        {
            List<SystemRoleDTO> roles = await roleServices.GetAllRolesAsync();
            return roles is null? RequestResult<List<SystemRoleDTO>>.Failed(ErrorCode.NoRolesFound): RequestResult<List<SystemRoleDTO>>.Success(roles);
        }
    }


}
