using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.AuthServiceIntegrationTests
{
    public class ResetPasswordIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ResetPasswordAsync_Should_Reset_Password()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                password: "OldPassword123!");

            var token = await GeneratePasswordResetTokenAsync(user);

            var service = GetService<AuthService>();

            var dto = new ResetPasswordDto
            {
                UserId = user.Id,
                Token = token,
                NewPassword = "NewPassword123!"
            };

            await service.ResetPasswordAsync(dto);

            var loginResult =
                await UserManager.CheckPasswordAsync(
                    user,
                    "NewPassword123!");

            loginResult.Should().BeTrue();
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_Clear_RefreshToken()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                password: "OldPassword123!");

            user.RefreshToken = "old-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);

            await UserManager.UpdateAsync(user);

            var token = await GeneratePasswordResetTokenAsync(user);

            var service = GetService<AuthService>();

            var dto = new ResetPasswordDto
            {
                UserId = user.Id,
                Token = token,
                NewPassword = "NewPassword123!"
            };

            await service.ResetPasswordAsync(dto);

            var updatedUser =
                await UserManager.FindByIdAsync(user.Id);

            updatedUser!.RefreshToken.Should().BeNull();
            updatedUser.RefreshTokenExpiryTime.Should().BeNull();
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_Throw_When_User_Not_Found()
        {
            var service = GetService<AuthService>();

            var dto = new ResetPasswordDto
            {
                UserId = Guid.NewGuid().ToString(),
                Token = "invalid",
                NewPassword = "NewPassword123!"
            };

            Func<Task> act = async () => await service.ResetPasswordAsync(dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_Throw_When_Token_Is_Invalid()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                password: "OldPassword123!");

            var service = GetService<AuthService>();

            var dto = new ResetPasswordDto
            {
                UserId = user.Id,
                Token = "aW52YWxpZA",
                NewPassword = "NewPassword123!"
            };

            Func<Task> act = async () => await service.ResetPasswordAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>();
        }
    }
}

