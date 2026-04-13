using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArteEva.Models
{
    public class Role : IdentityRole<int>
    {
        [MaxLength(200)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
