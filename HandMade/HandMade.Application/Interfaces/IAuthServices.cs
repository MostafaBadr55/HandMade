using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Interfaces
{
    public interface IAuthServices
    {
        /// <summary>
        /// Finds a domain <see cref="User"/> by email using the identity <see cref="UserManager{TUser}"/>.
        /// Returns <c>null</c> when no matching identity user is found.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The mapped domain <see cref="User"/> or <c>null</c> if not found.</returns>
        Task<User?> GetUserByEmailAsync(string email);
        /// <summary>
        /// Finds a domain <see cref="User"/> by username using the identity <see cref="UserManager{TUser}"/>.
        /// Returns <c>null</c> when no matching identity user is found.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The mapped domain <see cref="User"/> or <c>null</c> if not found.</returns>
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(Guid Id);
        /// <summary>
        /// Creates a new user account with the specified password asynchronously.
        /// </summary>
        /// <param name="user">The user information to create. Must not be null.</param>
        /// <param name="password">The password to associate with the new user. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
        /// <exception cref="Exception">Thrown if user creation fails due to validation errors or other issues.</exception>
        Task<RepoResult<User>> CreateAsync(User user, string password);

        Task<bool> UpdateUserAsync(User user);
        /// <summary>
        /// Retrieves the role names for the specified domain user.
        /// Finds the corresponding IdentityAppUser by the user's Id and returns
        /// the list of role names assigned to that identity user. Returns null
        /// if the identity user cannot be found.
        /// </summary>
        /// <param name="userId">The domain user Id whose roles to retrieve.</param>
        /// <returns>A list of role names, or null if the user is not found.</returns>
        Task<IList<string>> GetRolesAsync(Guid userId);
        /// <summary>
        /// Checks if the specified role exists in the system.
        /// </summary>
        /// <param name="role"></param>
        /// <returns>true if the role is found and false if the role not found in the system</returns>
        Task<bool> RoleExistsAsync(AssignedRole role);

        Task<bool> AddToRoleAsync(User user, AssignedRole role);
        /// <summary>
        /// Checks if the password matches the password of the given user.
        /// </summary>
        /// <param name="user">The domain user whose password to check.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns><c>true</c> if the password is correct; if incorrect <c>false</c>; if the user not found returns <c>null</c></returns>
        Task<bool?> CheckPasswordAsync(User user, string password);
    }
}
