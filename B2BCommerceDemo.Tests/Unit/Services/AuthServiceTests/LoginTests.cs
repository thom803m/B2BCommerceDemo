using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests
{
    public class LoginTests : AuthServiceTestBase
    {
        [Fact]
        public async Task LoginAsync_Should_Throw_When_Email_Not_Confirmed()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1));

            await context.SaveChangesAsync();

            var user = CreateUser(
                companyId: 1,
                emailConfirmed: false);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(user.Email!))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.CheckPasswordAsync(
                    user,
                    "Password123"))
                .ReturnsAsync(true);

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.LoginAsync(
                    new LoginDto
                    {
                        Email = user.Email!,
                        Password = "Password123"
                    });

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Email not confirmed.");
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Token()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1));

            await context.SaveChangesAsync();

            var user = CreateUser(
                email: "test@test.dk",
                companyId: 1);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync("test@test.dk"))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.CheckPasswordAsync(
                    user,
                    "Password123"))
                .ReturnsAsync(true);

            var jwtService = CreateJwtService();

            var service = CreateService(
                context,
                userManager: userManager,
                jwtService: jwtService);

            var result = await service.LoginAsync(
                new LoginDto
                {
                    Email = "test@test.dk",
                    Password = "Password123"
                });

            result.Token.Should().Be("jwt-token");
            result.CompanyId.Should().Be(1);
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_User_Not_Found()
        {
            var context = CreateContext();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.LoginAsync(
                    new LoginDto
                    {
                        Email = "missing@test.dk",
                        Password = "Password123"
                    });

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_Company_Not_Found()
        {
            var context = CreateContext();

            var user = CreateUser(companyId: 999);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    user.Email!))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.CheckPasswordAsync(
                    user,
                    "Password123"))
                .ReturnsAsync(true);

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.LoginAsync(
                    new LoginDto
                    {
                        Email = user.Email!,
                        Password = "Password123"
                    });

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password.");
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_Company_Is_Pending()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(
                    1,
                    status: CompanyStatus.Pending));

            await context.SaveChangesAsync();

            var user = CreateUser(companyId: 1);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    user.Email!))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.CheckPasswordAsync(
                    user,
                    "Password123"))
                .ReturnsAsync(true);

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.LoginAsync(
                    new LoginDto
                    {
                        Email = user.Email!,
                        Password = "Password123"
                    });

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Your company registration is awaiting approval.");
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_Password_Is_Invalid()
        {
            var context = CreateContext();

            context.Companies.Add(CreateCompany(1));

            await context.SaveChangesAsync();

            var user = CreateUser(companyId: 1);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    user.Email!))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.CheckPasswordAsync(
                    user,
                    "WrongPassword"))
                .ReturnsAsync(false);

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.LoginAsync(
                    new LoginDto
                    {
                        Email = user.Email!,
                        Password = "WrongPassword"
                    });

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password.");
        }

        [Fact]
        public async Task LoginAsync_Should_Call_JwtService()
        {
            var context = CreateContext();

            context.Companies.Add(CreateCompany(1));

            await context.SaveChangesAsync();

            var user = CreateUser(companyId: 1);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    user.Email!))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.CheckPasswordAsync(
                    user,
                    "Password123"))
                .ReturnsAsync(true);

            var jwtService = CreateJwtService();

            var service = CreateService(
                context,
                userManager: userManager,
                jwtService: jwtService);

            await service.LoginAsync(
                new LoginDto
                {
                    Email = user.Email!,
                    Password = "Password123"
                });

            jwtService.Verify(
                x => x.GenerateToken(user, 1),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_Should_Trim_Email()
        {
            var context = CreateContext();

            context.Companies.Add(CreateCompany(1));

            await context.SaveChangesAsync();

            var user = CreateUser(
                email: "test@test.dk",
                companyId: 1);

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    "test@test.dk"))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.CheckPasswordAsync(
                    user,
                    "Password123"))
                .ReturnsAsync(true);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.LoginAsync(
                new LoginDto
                {
                    Email = "  test@test.dk  ",
                    Password = "Password123"
                });

            userManager.Verify(
                x => x.FindByEmailAsync(
                    "test@test.dk"),
                Times.Once);
        }
    }
}
