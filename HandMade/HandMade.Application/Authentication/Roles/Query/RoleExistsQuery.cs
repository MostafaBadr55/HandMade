using HandMade.Application.Interfaces;
using HandMade.Domain.DomainEnums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Authentication.Roles.Query
{
    public record RoleExistsQuery(string username, AssignedRole role) : IRequest<bool>;

    public class RoleExistsQueryHandler(IAuthServices authServices) : IRequestHandler<RoleExistsQuery, bool>
    {
        public async Task<bool> Handle(RoleExistsQuery request, CancellationToken cancellationToken)
        {
            bool result = await authServices.RoleExistsAsync(request.role);
            return result;
        }
    }


}
