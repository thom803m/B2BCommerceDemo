using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.JwtServiceTests.Shared;
using FluentAssertions;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace B2BCommerceDemo.Tests.Unit.Services.JwtServiceTests
{
    public class GenerateTokenTests : JwtServiceTestBase
    {
        [Fact]
        public async Task GenerateToken_Should_Create_Token_With_Expected_Claims()
        {
            var user = CreateUser();

            var userManager = CreateUserManager();

            userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            var service = CreateService(
                userManager,
                CreateClock(),
                CreateConfig());

            var token = await service.GenerateToken(user, companyId: 5);

            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            jwt.Claims.Should().Contain(c => c.Type == "CompanyId" && c.Value == "5");
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "test@test.com");
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "1");
        }

        [Fact]
        public async Task GenerateToken_Should_Not_Include_CompanyId_When_Null()
        {
            var user = CreateUser();

            var userManager = CreateUserManager();

            userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            var service = CreateService(
                userManager,
                CreateClock(),
                CreateConfig());

            var token = await service.GenerateToken(user, null);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.Claims.Should().NotContain(c => c.Type == "CompanyId");
        }

        [Fact]
        public async Task GenerateToken_Should_Throw_When_Email_Is_Missing()
        {
            var user = new ApplicationUser
            {
                Id = "1",
                Email = null
            };

            var userManager = CreateUserManager();

            userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            var service = CreateService(
                userManager,
                CreateClock(),
                CreateConfig());

            Func<Task> act = async () =>
                await service.GenerateToken(user, null);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("User 1 has no email.");
        }

        [Fact]
        public async Task GenerateToken_Should_Include_Multiple_Roles()
        {
            var user = CreateUser();

            var userManager = CreateUserManager();

            userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin", "User" });

            var service = CreateService(
                userManager,
                CreateClock(),
                CreateConfig());

            var token = await service.GenerateToken(user, null);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .Should().BeEquivalentTo("Admin", "User");
        }

        [Fact]
        public async Task GenerateToken_Should_Use_Clock_For_Expiry()
        {
            var user = CreateUser();

            var userManager = CreateUserManager();

            userManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            var clock = CreateClock();

            var service = CreateService(
                userManager,
                clock,
                CreateConfig());

            var token = await service.GenerateToken(user, null);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.ValidTo.Should().Be(clock.UtcNow.AddHours(8));
        }
    }
}
