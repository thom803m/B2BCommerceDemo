using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.AuthServiceIntegrationTests
{
    public class LoginIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task LoginAsync_Should_Return_Token_For_Valid_User()
        {
            var service = GetService<AuthService>();

            var user = await CreateUserAsync(
                email: "test@test.dk",
                password: "Test123!",
                emailConfirmed: true);

            var dto = new LoginDto
            {
                Email = "test@test.dk",
                Password = "Test123!"
            };

            var result = await service.LoginAsync(dto);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();

            var updatedUser =
                await UserManager.FindByEmailAsync(dto.Email);

            updatedUser!.RefreshToken.Should().NotBeNull();
            updatedUser.RefreshTokenExpiryTime.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_User_Not_Found()
        {
            var service = GetService<AuthService>();

            var dto = new LoginDto
            {
                Email = "missing@test.dk",
                Password = "Test123!"
            };

            Func<Task> act = async () => await service.LoginAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password.");
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_Email_Not_Confirmed()
        {
            await CreateUserAsync(
                email: "test@test.dk",
                password: "Test123!",
                emailConfirmed: false);

            var service = GetService<AuthService>();

            var dto = new LoginDto
            {
                Email = "test@test.dk",
                Password = "Test123!"
            };

            Func<Task> act = async () => await service.LoginAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Email not confirmed.");
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_Password_Is_Invalid()
        {
            await CreateUserAsync(
                email: "test@test.dk",
                password: "Test123!");

            var service = GetService<AuthService>();

            var dto = new LoginDto
            {
                Email = "test@test.dk",
                Password = "WrongPassword123!"
            };

            Func<Task> act = async () => await service.LoginAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password.");
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_Company_Is_Not_Active()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Pending
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            await CreateUserAsync(
                email: "test@test.dk",
                password: "Test123!",
                emailConfirmed: true,
                companyId: company.Id);

            var service = GetService<AuthService>();

            var dto = new LoginDto
            {
                Email = "test@test.dk",
                Password = "Test123!"
            };

            Func<Task> act = async () => await service.LoginAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Company awaiting approval.");
        }
    }
}

