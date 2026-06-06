using HandMade.Application.CQRS.RolesManagement.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Domain.Entities;
using HandMade.Infrastructure.Identity.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Infrastructure.Identity
{
    public class RolesServices(RoleManager<IdentityAppRole> roleManager): IRoleServices
    {
        public async Task<List<SystemRoleDTO>> GetAllRolesAsync()
        {
            List<SystemRoleDTO> systemRoles = roleManager.Roles
                .Select(role => SystemRoleDTO.Create(role.Name, role.Description))
                .ToList();
            return systemRoles;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            return roleExists;
        }

        public async Task<bool> AddRoleAsync(CreateRoleDTO dto)
        {
            var role = new IdentityAppRole
            {
                Name = dto.name,
                Description = dto.description
            };

            var result = await roleManager.CreateAsync(role);

            return result.Succeeded;
        }
    }
}
