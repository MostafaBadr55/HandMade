using HandMade.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HandMade.Infrastructure.Identity.IdentityModels
{
    public class IdentityAppUserRole: IdentityUserRole<Guid>
    {
        // Navigation Properties
        public IdentityAppUser User { get; set; }
        public Role Role { get; set; }
    }
}
