using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.AuthServiceIntegrationTests
{
    public class RefreshTokenIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task RefreshTokenAsync_Should_Return_New_Tokens()
        {
            var user = await CreateUserAsync();

            user.RefreshToken = "old-refresh-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            await UserManager.UpdateAsync(user);

            var service = GetService<AuthService>();

            var dto = new RefreshTokenRequestDto
            {
                RefreshToken = "old-refresh-token"
            };

            var result = await service.RefreshTokenAsync(dto);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBe("old-refresh-token");

            var updatedUser =
                await UserManager.FindByIdAsync(user.Id);

            updatedUser!.RefreshToken.Should()
                .Be(result.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_When_Token_Not_Found()
        {
            var service = GetService<AuthService>();

            var dto = new RefreshTokenRequestDto
            {
                RefreshToken = "does-not-exist"
            };

            Func<Task> act = async () => await service.RefreshTokenAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid refresh token.");
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_When_Token_Expired()
        {
            var user = await CreateUserAsync();

            user.RefreshToken = "expired-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1);

            await UserManager.UpdateAsync(user);

            var service = GetService<AuthService>();

            var dto = new RefreshTokenRequestDto
            {
                RefreshToken = "expired-token"
            };

            Func<Task> act = async () => await service.RefreshTokenAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Refresh token has expired.");
        }
    }
}

