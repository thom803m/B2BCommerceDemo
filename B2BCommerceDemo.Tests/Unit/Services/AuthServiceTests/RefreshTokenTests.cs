using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests
{
    public class RefreshTokenTests : AuthServiceTestBase
    {
        [Fact]
        public async Task RefreshTokenAsync_Should_Return_New_AccessToken()
        {
            var context = CreateContext();

            var user = CreateUser(
                companyId: 1);

            user.RefreshToken = "refresh-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            context.Users.Add(user);

            await context.SaveChangesAsync();

            var jwtService = CreateJwtService();

            var service = CreateService(
                context,
                jwtService: jwtService);

            var result = await service.RefreshTokenAsync(
                new RefreshTokenRequestDto
                {
                    RefreshToken = "refresh-token"
                });

            result.Token.Should().Be("jwt-token");
            result.CompanyId.Should().Be(1);
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_When_RefreshToken_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.RefreshTokenAsync(
                    new RefreshTokenRequestDto
                    {
                        RefreshToken = "invalid-token"
                    });

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid refresh token.");
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_When_RefreshToken_Is_Expired()
        {
            var context = CreateContext();

            var user = CreateUser(
                companyId: 1);

            user.RefreshToken = "refresh-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1);

            context.Users.Add(user);

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.RefreshTokenAsync(
                    new RefreshTokenRequestDto
                    {
                        RefreshToken = "refresh-token"
                    });

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Refresh token has expired.");
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Generate_New_RefreshToken()
        {
            var context = CreateContext();

            var user = CreateUser(
                companyId: 1);

            user.RefreshToken = "old-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            context.Users.Add(user);

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.RefreshTokenAsync(
                new RefreshTokenRequestDto
                {
                    RefreshToken = "old-token"
                });

            result.RefreshToken.Should().NotBe("old-token");

            var updatedUser = await context.Users
                .FirstAsync(x => x.Id == user.Id);

            updatedUser.RefreshToken.Should().Be(result.RefreshToken);
        }
    }
}

