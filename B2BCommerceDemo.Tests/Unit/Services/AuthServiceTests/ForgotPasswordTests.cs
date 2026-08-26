using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Events.Users;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests
{
    public class ForgotPasswordTests : AuthServiceTestBase
    {
        [Fact]
        public async Task ForgotPasswordAsync_Should_Publish_Reset_Event()
        {
            var context = CreateContext();

            var dispatcher = CreateEventDispatcher();

            var user = CreateUser();
            user.EmailConfirmed = true;

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(user.Email!))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token");

            var service = CreateService(
                context,
                userManager: userManager,
                eventDispatcher: dispatcher);

            await service.ForgotPasswordAsync(
                new ForgotPasswordDto
                {
                    Email = user.Email!
                });

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<PasswordResetRequestedEvent>(e =>
                        e.UserId == user.Id &&
                        e.Email == user.Email &&
                        e.Token == "reset-token")),
                Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordAsync_Should_Return_When_User_Not_Found()
        {
            var context = CreateContext();

            var dispatcher = CreateEventDispatcher();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(
                context,
                userManager: userManager,
                eventDispatcher: dispatcher);

            await service.ForgotPasswordAsync(
                new ForgotPasswordDto
                {
                    Email = "missing@test.dk"
                });

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.IsAny<PasswordResetRequestedEvent>()),
                Times.Never);
        }

        [Fact]
        public async Task ForgotPasswordAsync_Should_Return_When_Email_Not_Confirmed()
        {
            var context = CreateContext();

            var dispatcher = CreateEventDispatcher();

            var user = CreateUser();
            user.EmailConfirmed = false;

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    user.Email!))
                .ReturnsAsync(user);

            var service = CreateService(
                context,
                userManager: userManager,
                eventDispatcher: dispatcher);

            await service.ForgotPasswordAsync(
                new ForgotPasswordDto
                {
                    Email = user.Email!
                });

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.IsAny<PasswordResetRequestedEvent>()),
                Times.Never);
        }
    }
}

