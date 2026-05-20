using HandMade.Domain.Entities;
using HandMade.Infrastructure.Identity.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Infrastructure.Helpers
{
    public static class DataSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<IdentityAppUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityAppRole>>();

            string superAdminEmail = "admin@admin.com";
            string superAdminUsername = "superadmin"; // <-- login username
            string superAdminPassword = "Admin@123*";

            // Create roles if they don't exist
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await roleManager.CreateAsync(new IdentityAppRole
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Description = "Super administrator with full system access",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityAppRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Description = "Administrator with full system access",
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Check if super admin already exists
            var superAdmin = await userManager.FindByNameAsync(superAdminUsername);

            if (superAdmin == null)
            {
                var newSuperAdmin = new IdentityAppUser()
                {
                    UserName = superAdminUsername, // <-- username for login
                    Email = superAdminEmail,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(newSuperAdmin, superAdminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create SuperAdmin user: {errors}");
                }
            }
        }
    }
}
