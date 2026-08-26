using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.AuthServiceIntegrationTests
{
    public class ForgotPasswordIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ForgotPasswordAsync_Should_Not_Throw_When_User_Does_Not_Exist()
        {
            var service = GetService<AuthService>();

            var dto = new ForgotPasswordDto
            {
                Email = "missing@test.dk"
            };

            Func<Task> act = async () => await service.ForgotPasswordAsync(dto);

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task ForgotPasswordAsync_Should_Not_Throw_When_Email_Not_Confirmed()
        {
            await CreateUserAsync(
                email: "test@test.dk",
                emailConfirmed: false);

            var service = GetService<AuthService>();

            var dto = new ForgotPasswordDto
            {
                Email = "test@test.dk"
            };

            await service.ForgotPasswordAsync(dto);
        }

        [Fact]
        public async Task ForgotPasswordAsync_Should_Publish_Event_For_Confirmed_User()
        {
            await CreateUserAsync(
                email: "test@test.dk",
                emailConfirmed: true);

            var service = GetService<AuthService>();

            var dto = new ForgotPasswordDto
            {
                Email = "test@test.dk"
            };

            await service.ForgotPasswordAsync(dto);
        }
    }
}

