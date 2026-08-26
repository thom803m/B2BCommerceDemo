using B2BCommerceDemo.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace B2BCommerceDemo.Infrastructure.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            string? configuredEmail,
            string? configuredPassword)
        {
            var email = configuredEmail?.Trim();
            var password = configuredPassword;

            if (
                string.IsNullOrWhiteSpace(email) &&
                string.IsNullOrWhiteSpace(password)
            )
            {
                return;
            }

            if (
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password)
            )
            {
                throw new InvalidOperationException("Both AdminBootstrap:Email and AdminBootstrap:Password must be configured.");
            }

            var existing = await userManager.FindByEmailAsync(email);

            if (existing != null)
            {
                return;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var createResult =
                await userManager.CreateAsync(
                    user,
                    password
                );

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    " ",
                    createResult.Errors.Select(
                        error => error.Description
                    )
                );

                throw new InvalidOperationException(
                    $"Failed to seed admin user. {errors}"
                );
            }

            var roleResult =
                await userManager.AddToRoleAsync(
                    user,
                    "Admin"
                );

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    " ",
                    roleResult.Errors.Select(
                        error => error.Description
                    )
                );

                throw new InvalidOperationException(
                    $"Failed to assign the Admin role. {errors}"
                );
            }
        }
    }
}
