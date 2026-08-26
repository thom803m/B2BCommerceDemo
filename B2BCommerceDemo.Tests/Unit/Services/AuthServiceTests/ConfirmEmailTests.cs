using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests
{
    public class ConfirmEmailTests : AuthServiceTestBase
    {
        [Fact]
        public async Task ConfirmEmailAsync_Should_Confirm_Email()
        {
            var context = CreateContext();

            var user = CreateUser();
            user.EmailConfirmed = false;

            var userManager = CreateUserManager();

            var token = "reset-token";
            var encodedToken = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
                System.Text.Encoding.UTF8.GetBytes(token));

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.ConfirmEmailAsync(
                user.Id,
                encodedToken);

            userManager.Verify(
                x => x.ConfirmEmailAsync(user, token),
                Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailAsync_Should_Throw_When_User_Not_Found()
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
                await service.ConfirmEmailAsync(
                    "user1",
                    "token");

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task ConfirmEmailAsync_Should_Return_When_Email_Already_Confirmed()
        {
            var context = CreateContext();

            var user = CreateUser();
            user.EmailConfirmed = true;

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.ConfirmEmailAsync(
                user.Id,
                "token");

            userManager.Verify(
                x => x.ConfirmEmailAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()),
                Times.Never);
        }
    }
}

