using HandMade.Application.CQRS.RolesManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Interfaces
{
    public interface IRoleServices
    {
        /// <summary>
        /// Retrieves a list of all roles available in the system.
        /// </summary>
        /// <returns>A list of roles.</returns>
        Task<List<SystemRoleDTO>> GetAllRolesAsync();
        /// <summary>
        /// Checks if a role with the specified name exists in the system.
        /// </summary>
        /// <param name="roleName">The name of the role to check.</param>
        /// <returns>True if the role exists, otherwise false.</returns>
        Task<bool> RoleExistsAsync(string roleName);
        /// <summary>
        /// Adds a new role to the system.
        /// </summary>
        /// <param name="dto">The data transfer object containing role information.</param>
        /// <returns>True if the role was successfully added, otherwise false.</returns>
        Task<bool> AddRoleAsync(CreateRoleDTO dto);
    }
}
