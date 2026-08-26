using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace B2BCommerceDemo.Infrastructure.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider services
        )
        {
            var context =
                services.GetRequiredService<AppDbContext>();

            var roleManager =
                services.GetRequiredService<
                    RoleManager<IdentityRole>
                >();

            var userManager =
                services.GetRequiredService<
                    UserManager<ApplicationUser>
                >();

            var configuration =
                services.GetRequiredService<
                    IConfiguration
                >();

            await RoleSeeder.SeedAsync(
                roleManager
            );

            await PriceGroupSeeder.SeedAsync(
                context
            );

            await AdminSeeder.SeedAsync(
                userManager,
                configuration[
                    "AdminBootstrap:Email"
                ],
                configuration[
                    "AdminBootstrap:Password"
                ]
            );
        }
    }
}
