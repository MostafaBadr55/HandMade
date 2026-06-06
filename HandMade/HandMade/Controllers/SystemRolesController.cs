using HandMade.Application.CQRS.RolesManagement.DTOs;
using HandMade.Application.CQRS.RolesManagement.Orchesterator;
using HandMade.Application.CQRS.RolesManagement.Queries;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels;
using HandMade.ViewModels.SystemRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{nameof(AssignedRole.Admin)},SuperAdmin")]
    public class SystemRolesController(IMediator mediator) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<ActionResult<List<SystemRolesVM>>> GetAllSystemRoles()
        {
            RequestResult<List<SystemRoleDTO>> roles = await mediator.Send(new GetSytemRolesQuery());
            if (!roles.IsSuccess)
                return roles.ErrorCode.ToProblem("No roles found");

            List<SystemRolesVM> rolesList = roles.Data.Select(role => SystemRolesVM.Create(role.Name, role.Description)).ToList();
            return rolesList;
        }

        [HttpPost]
        public async Task<ActionResult<AddRoleResponseVM>> AddRole(AddRoleRequestVM request)
        {
            if (!ModelState.IsValid)
                return ErrorCode.InvalidRoleName.ToProblem("Invalid Role Name");
            var response = await mediator.Send(new CreateNewSystemRoleOrchesterator(request.Role.ToString(), request.Description));

            return response.IsSuccess ? Created(string.Empty, new AddRoleResponseVM { Name = request.Role.ToString() }) : response.ErrorCode.ToProblem("role Creation Failed");

        }
    }
}
