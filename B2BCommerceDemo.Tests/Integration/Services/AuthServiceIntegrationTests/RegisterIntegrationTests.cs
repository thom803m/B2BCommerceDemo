using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.AuthServiceIntegrationTests
{
    public class RegisterIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task RegisterAsync_Should_Create_User_And_Company()
        {
            var service = GetService<AuthService>();

            var dto = new RegisterDto
            {
                CompanyName = "Test Company",
                Email = "test@test.dk",
                Password = "Test123!"
            };

            await service.RegisterAsync(dto);

            Context.Companies.Should().HaveCount(1);
            Context.Users.Should().HaveCount(1);

            var company = Context.Companies.Single();

            company.Name.Should().Be("Test Company");
            company.Status.Should().Be(CompanyStatus.Pending);

            var user = await UserManager.FindByEmailAsync("test@test.dk");

            user.Should().NotBeNull();
            user!.CompanyId.Should().Be(company.Id);
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_When_Email_Already_Exists()
        {
            await CreateUserAsync();

            var service = GetService<AuthService>();

            var dto = new RegisterDto
            {
                CompanyName = "New Company",
                Email = "test@test.dk",
                Password = "Test123!"
            };

            Func<Task> act = async () => await service.RegisterAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("User with this email already exists.");
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_When_Company_Name_Already_Exists()
        {
            Context.Companies.Add(new Company
            {
                Name = "Existing Company",
                Status = CompanyStatus.Active
            });

            await Context.SaveChangesAsync();

            var service = GetService<AuthService>();

            var dto = new RegisterDto
            {
                CompanyName = "Existing Company",
                Email = "new@test.dk",
                Password = "Test123!"
            };

            Func<Task> act = async () => await service.RegisterAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task RegisterAsync_Should_Assign_User_Role()
        {
            var service = GetService<AuthService>();

            var dto = new RegisterDto
            {
                CompanyName = "Test Company",
                Email = "test@test.dk",
                Password = "Test123!"
            };

            await service.RegisterAsync(dto);

            var user = await UserManager.FindByEmailAsync(dto.Email);

            var roles = await UserManager.GetRolesAsync(user!);

            roles.Should().Contain("User");
        }
    }
}

