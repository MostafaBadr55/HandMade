using HandMade.Application.Shared;
using HandMade.Domain.Entities;

namespace HandMade.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(Guid Id);
        Task<RepoResult<User>> CreateAsync(User user, string password);
        Task<bool?> CheckPasswordAsync(User user, string password);
        Task<IList<string>> GetRolesAsync(Guid userId);
    }
}
