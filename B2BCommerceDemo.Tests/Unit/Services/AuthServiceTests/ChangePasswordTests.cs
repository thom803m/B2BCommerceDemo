using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests
{
    public class ChangePasswordTests : AuthServiceTestBase
    {
        [Fact]
        public async Task ChangePasswordAsync_Should_Change_Password()
        {
            var context = CreateContext();

            var user = CreateUser();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123",
                    "NewPassword123"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.ChangePasswordAsync(
                user.Id,
                new ChangePasswordDto
                {
                    CurrentPassword = "OldPassword123",
                    NewPassword = "NewPassword123"
                });

            userManager.Verify(
                x => x.ChangePasswordAsync(
                    user,
                    "OldPassword123",
                    "NewPassword123"),
                Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Throw_When_User_Not_Found()
        {
            var context = CreateContext();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByIdAsync("missing"))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.ChangePasswordAsync(
                    "missing",
                    new ChangePasswordDto());

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Throw_When_Current_Password_Is_Invalid()
        {
            var context = CreateContext();

            var user = CreateUser();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Incorrect password."
                        }));

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.ChangePasswordAsync(
                    user.Id,
                    new ChangePasswordDto());

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Incorrect password.*");
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Clear_RefreshToken()
        {
            var context = CreateContext();

            var user = CreateUser();

            user.RefreshToken = "refresh-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.ChangePasswordAsync(
                    user,
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.ChangePasswordAsync(
                user.Id,
                new ChangePasswordDto
                {
                    CurrentPassword = "OldPassword123",
                    NewPassword = "NewPassword123"
                });

            user.RefreshToken.Should().BeNull();
            user.RefreshTokenExpiryTime.Should().BeNull();
        }
    }
}

