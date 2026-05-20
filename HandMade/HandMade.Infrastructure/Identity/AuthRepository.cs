using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using HandMade.Infrastructure.Identity.IdentityModels;
using Microsoft.AspNetCore.Identity;


namespace HandMade.Infrastructure.Identity
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<IdentityAppUser> _userManager;
        private readonly SignInManager<IdentityAppUser> _signInManager;

        public AuthRepository(
            UserManager<IdentityAppUser> userManager,
            SignInManager<IdentityAppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        /// <summary>
        /// Finds a domain <see cref="User"/> by email using the identity <see cref="UserManager{TUser}"/>.
        /// Returns <c>null</c> when no matching identity user is found.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The mapped domain <see cref="User"/> or <c>null</c> if not found.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            IdentityAppUser userFromDb = await _userManager.FindByEmailAsync(email);
            return userFromDb is null ? null : MapToDomain(userFromDb);
        }

        /// <summary>
        /// Finds a domain <see cref="User"/> by username using the identity <see cref="UserManager{TUser}"/>.
        /// Returns <c>null</c> when no matching identity user is found.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The mapped domain <see cref="User"/> or <c>null</c> if not found.</returns>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            IdentityAppUser? user = await _userManager.FindByNameAsync(username);
            return user is null ? null : MapToDomain(user);
        }
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            string userid = id.ToString();
            IdentityAppUser? user = await _userManager.FindByIdAsync(userid);

            return user is null? null: MapToDomain(user);
        }
        
        /// <summary>
        /// Retrieves the role names for the specified domain user.
        /// Finds the corresponding IdentityAppUser by the user's Id and returns
        /// the list of role names assigned to that identity user. Returns null
        /// if the identity user cannot be found.
        /// </summary>
        /// <param name="user">The domain user whose roles to retrieve.</param>
        /// <returns>A list of role names, or null if the user is not found.</returns>
        public async Task<IList<string>> GetRolesAsync(Guid userId)
        {
            IdentityAppUser? identityUser = await _userManager.FindByIdAsync(userId.ToString());

            if (identityUser is null) return null;

            return await _userManager.GetRolesAsync(identityUser);
        }
        /// <summary>
        /// Checks if the password matches the password of the given user.
        /// </summary>
        /// <param name="user">The domain user whose password to check.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns><c>true</c> if the password is correct; if incorrect <c>false</c>; if the user not found returns <c>null</c></returns>
        public async Task<bool?> CheckPasswordAsync(User user, string password)
        {
            var identityUser = await _userManager.FindByNameAsync(user.UserName);

            if (identityUser is null) return null;

            var result = await _signInManager
                .CheckPasswordSignInAsync(identityUser, password, lockoutOnFailure: false);

            return result.Succeeded? true : false;
        }
        /// <summary>
        /// Creates a new user account with the specified password asynchronously.
        /// </summary>
        /// <param name="user">The user information to create. Must not be null.</param>
        /// <param name="password">The password to associate with the new user. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
        /// <exception cref="Exception">Thrown if user creation fails due to validation errors or other issues.</exception>
        public async Task<RepoResult<User>> CreateAsync(User user, string password)
        {
            IdentityAppUser identityUser = MapToIdentity(user);

            IdentityResult result = await _userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                List<string> errors = (List<string>)result.Errors.Select(e => e.Description);
                return RepoResult<User>.Failure(errors); // ← no throw
            }

            return RepoResult<User>.Success(MapToDomain(identityUser));
        }


        // -------------------------
        // Mapping
        // -------------------------

        private static User MapToDomain(IdentityAppUser i) => new()
        {
            Email = i.Email!,
            UserName = i.UserName!,
            PhoneNumber = i.PhoneNumber,
            IsSeller = i.IsSeller,
            IsActive = i.IsActive,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };

        private static IdentityAppUser MapToIdentity(User u) => new()
        {
            Id = u.Id == Guid.Empty ? Guid.NewGuid() : u.Id,
            Email = u.Email,
            UserName = u.UserName,
            PhoneNumber = u.PhoneNumber,
            IsSeller = u.IsSeller,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        };

    }
}
