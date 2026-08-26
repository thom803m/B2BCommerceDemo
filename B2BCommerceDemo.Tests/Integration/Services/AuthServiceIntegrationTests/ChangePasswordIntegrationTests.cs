using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.AuthServiceIntegrationTests
{
    public class ChangePasswordIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ChangePasswordAsync_Should_Change_Password()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                password: "OldPassword123!");

            var service = GetService<AuthService>();

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            await service.ChangePasswordAsync(user.Id, dto);

            var passwordValid =
                await UserManager.CheckPasswordAsync(
                    user,
                    "NewPassword123!");

            passwordValid.Should().BeTrue();
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Clear_RefreshToken()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                password: "OldPassword123!");

            user.RefreshToken = "refresh-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

            await UserManager.UpdateAsync(user);

            var service = GetService<AuthService>();

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            await service.ChangePasswordAsync(user.Id, dto);

            var updatedUser =
                await UserManager.FindByIdAsync(user.Id);

            updatedUser!.RefreshToken.Should().BeNull();
            updatedUser.RefreshTokenExpiryTime.Should().BeNull();
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Throw_When_CurrentPassword_Is_Invalid()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                password: "OldPassword123!");

            var service = GetService<AuthService>();

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "WrongPassword123!",
                NewPassword = "NewPassword123!"
            };

            var act = () =>
                service.ChangePasswordAsync(user.Id, dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Throw_When_User_Not_Found()
        {
            var service = GetService<AuthService>();

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var act = () =>
                service.ChangePasswordAsync(
                    Guid.NewGuid().ToString(),
                    dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("User not found");
        }
    }
}

