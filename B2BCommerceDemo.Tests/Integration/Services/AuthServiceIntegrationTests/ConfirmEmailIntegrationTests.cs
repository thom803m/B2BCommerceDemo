using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.AuthServiceIntegrationTests
{
    public class ConfirmEmailIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ConfirmEmailAsync_Should_Confirm_Email()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                emailConfirmed: false);

            var token =
                await GenerateEmailConfirmationTokenAsync(user);

            var service = GetService<AuthService>();

            await service.ConfirmEmailAsync(
                user.Id,
                token);

            var updatedUser =
                await UserManager.FindByIdAsync(user.Id);

            updatedUser!.EmailConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task ConfirmEmailAsync_Should_Throw_When_User_Not_Found()
        {
            var service = GetService<AuthService>();

            var act = () =>
                service.ConfirmEmailAsync(
                    Guid.NewGuid().ToString(),
                    "invalid");

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task ConfirmEmailAsync_Should_Return_When_Email_Already_Confirmed()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                emailConfirmed: true);

            var service = GetService<AuthService>();

            var act = () =>
                service.ConfirmEmailAsync(
                    user.Id,
                    "anything");

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ConfirmEmailAsync_Should_Throw_When_Token_Is_Invalid()
        {
            var user = await CreateUserAsync(
                email: "test@test.dk",
                emailConfirmed: false);

            var service = GetService<AuthService>();

            var act = () =>
                service.ConfirmEmailAsync(
                    user.Id,
                    "aW52YWxpZA");

            await act.Should()
                .ThrowAsync<InvalidOperationException>();
        }
    }
}

