using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.JwtServiceTests.Shared
{
    public abstract class JwtServiceTestBase
    {
        protected static Mock<UserManager<ApplicationUser>> CreateUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!, null!, null!, null!, null!, null!, null!, null!);
        }

        protected static IClock CreateClock()
        {
            var clock = new Mock<IClock>();

            clock.Setup(x => x.UtcNow)
                .Returns(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            return clock.Object;
        }

        protected static IConfiguration CreateConfig()
        {
            var dict = new Dictionary<string, string>
            {
                ["Jwt:Key"] = "THIS_IS_A_SUPER_LONG_TEST_KEY_FOR_HS256_SIGNING_123456",
                ["Jwt:Issuer"] = "test_issuer",
                ["Jwt:Audience"] = "test_audience"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(dict!)
                .Build();
        }

        protected static JwtService CreateService(
            Mock<UserManager<ApplicationUser>> userManager,
            IClock clock,
            IConfiguration config)
        {
            return new JwtService(config, userManager.Object, clock);
        }

        protected static ApplicationUser CreateUser(
            string id = "1",
            string email = "test@test.com")
        {
            return new ApplicationUser
            {
                Id = id,
                Email = email
            };
        }
    }
}
