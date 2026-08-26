using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System.Text;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests
{
    public class ResetPasswordTests : AuthServiceTestBase
    {
        [Fact]
        public async Task ResetPasswordAsync_Should_Reset_Password()
        {
            var context = CreateContext();

            var user = CreateUser();

            var userManager = CreateUserManager();

            var token = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes("reset-token"));

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.ResetPasswordAsync(
                    user,
                    "reset-token",
                    "NewPassword123"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.ResetPasswordAsync(
                new ResetPasswordDto
                {
                    UserId = user.Id,
                    Token = token,
                    NewPassword = "NewPassword123"
                });

            userManager.Verify(
                x => x.ResetPasswordAsync(
                    user,
                    "reset-token",
                    "NewPassword123"),
                Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_Throw_When_User_Not_Found()
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
                await service.ResetPasswordAsync(
                    new ResetPasswordDto
                    {
                        UserId = "missing",
                        Token = "token",
                        NewPassword = "Password123"
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_Throw_When_Token_Is_Invalid()
        {
            var context = CreateContext();

            var user = CreateUser();

            var userManager = CreateUserManager();

            var token = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes("invalid-token"));

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.ResetPasswordAsync(
                    user,
                    "invalid-token",
                    "NewPassword123"))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Invalid token"
                        }));

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.ResetPasswordAsync(
                    new ResetPasswordDto
                    {
                        UserId = user.Id,
                        Token = token,
                        NewPassword = "NewPassword123"
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid token");
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_Clear_RefreshToken()
        {
            var context = CreateContext();

            var user = CreateUser();

            user.RefreshToken = "old-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

            var userManager = CreateUserManager();

            var token = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes("reset-token"));

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.ResetPasswordAsync(
                    user,
                    "reset-token",
                    "NewPassword123"))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.ResetPasswordAsync(
                new ResetPasswordDto
                {
                    UserId = user.Id,
                    Token = token,
                    NewPassword = "NewPassword123"
                });

            user.RefreshToken.Should().BeNull();
            user.RefreshTokenExpiryTime.Should().BeNull();

            userManager.Verify(
                x => x.UpdateAsync(user),
                Times.Once);
        }
    }
}

