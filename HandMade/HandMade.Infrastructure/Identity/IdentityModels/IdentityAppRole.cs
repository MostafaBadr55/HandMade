using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Infrastructure.Identity.IdentityModels
{
    public class IdentityAppRole : IdentityRole<Guid>
    {
        [MaxLength(200)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<IdentityAppUserRole> UserRoles { get; set; }
    }
}
