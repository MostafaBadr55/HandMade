using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Interfaces
{
    public interface IAuthTokenService
    {
        Task<string> GenerateTokenAsync(Guid userId, string userName, string email, IList<string> roles);
    }
}
