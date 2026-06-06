using Azure.Core;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using HandMade.Infrastructure.Identity.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace HandMade.Infrastructure.Identity
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<IdentityAppUser> _userManager;
        private readonly RoleManager<IdentityAppRole> _roleManager;
        private readonly SignInManager<IdentityAppUser> _signInManager;

        public AccountServices(
            UserManager<IdentityAppUser> userManager,
            RoleManager<IdentityAppRole> roleManager,
            SignInManager<IdentityAppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

        }
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            IdentityAppUser userFromDb = await _userManager.FindByEmailAsync(email);
            return userFromDb is null ? null : MapToDomain(userFromDb);
        }

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
        public async Task<IList<string>> GetRolesAsync(Guid userId)
        {
            IdentityAppUser? identityUser = await _userManager.FindByIdAsync(userId.ToString());

            if (identityUser is null) return null;

            return await _userManager.GetRolesAsync(identityUser);
        }

        public async Task<bool> RoleExistsAsync(AssignedRole role)
        {
            bool exists = await _roleManager.RoleExistsAsync(role.ToString());
            return exists;
        }

        public async Task<bool> AddToRoleAsync(string userId, AssignedRole role)
        {
            IdentityAppUser? identityUser = await _userManager.FindByIdAsync(userId);
            if (identityUser is null)
                return false;

            IdentityResult result = await _userManager.AddToRoleAsync(identityUser, role.ToString());
            return result.Succeeded;
        }
        public async Task<RepoResult<User>> CreateAsync(User user, string password)
        {
            IdentityAppUser identityUser = MapToIdentity(user);

            IdentityResult result = await _userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                List<string> errors = (List<string>)result.Errors.Select(e => e.Description).ToList();
                return RepoResult<User>.Failure(errors); 
            }

            return RepoResult<User>.Success(MapToDomain(identityUser));
        }
        public async Task<bool> UpdateUserAsync(User user)
        {
            IdentityAppUser? identityUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (identityUser is null)
                return false;

            identityUser.UserName = user.UserName;
            identityUser.Email = user.Email;
            identityUser.PhoneNumber = user.PhoneNumber;
            identityUser.IsActive = user.IsActive;
            identityUser.IsSeller = user.IsSeller;
            identityUser.UpdatedAt = user.UpdatedAt;

            IdentityResult result = await _userManager.UpdateAsync(identityUser);
            return result.Succeeded;
        }
        public async Task<bool?> CheckPasswordAsync(User user, string password)
        {
            var identityUser = await _userManager.FindByNameAsync(user.UserName);

            if (identityUser is null) return null;

            var result = await _signInManager
                .CheckPasswordSignInAsync(identityUser, password, lockoutOnFailure: false);

            return result.Succeeded? true : false;
        }

        public async Task<Dictionary<Guid, string>> GetUsernamesByIdsAsync(IEnumerable<Guid> userIds)
        {
            var users = await _userManager.Users
                                .Where(u => userIds.Contains(u.Id))
                                .Select(u => new { u.Id, u.UserName })
                                .ToListAsync();

            return users.ToDictionary(u => u.Id, u => u.UserName ?? string.Empty);
        }


        // -------------------------
        // Mapping
        // -------------------------

        private static User MapToDomain(IdentityAppUser i) => new()
        {
            Id = i.Id, 
            Email = i.Email!,
            UserName = i.UserName!,
            PhoneNumber = i.PhoneNumber,
            IsSeller = i.IsSeller,
            IsActive = i.IsActive,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
            SecurityStamp = i.SecurityStamp!
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
            UpdatedAt = u.UpdatedAt,
            SecurityStamp = u.SecurityStamp!
        };

    }
}
