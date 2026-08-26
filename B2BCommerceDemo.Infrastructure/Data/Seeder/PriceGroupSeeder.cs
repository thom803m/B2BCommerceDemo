using B2BCommerceDemo.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Data.Seeder
{
    public static class PriceGroupSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.PriceGroups.AnyAsync())
                return;

            context.PriceGroups.AddRange(
                new PriceGroup
                {
                    Name = "Group A",
                    PercentageAdjustment = 0
                },
                new PriceGroup
                {
                    Name = "Group B",
                    PercentageAdjustment = 5
                },
                new PriceGroup
                {
                    Name = "Group C",
                    PercentageAdjustment = 10
                });

            await context.SaveChangesAsync();
        }
    }
}

