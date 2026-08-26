using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Events.Users;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests
{
    public class RegisterTests : AuthServiceTestBase
    {
        [Fact]
        public async Task RegisterAsync_Should_Create_Company()
        {
            var context = CreateContext();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "test@test.dk",
                    Password = "Password123!",
                    CompanyName = "Company A"
                });

            context.Companies.Should().ContainSingle();

            var company = context.Companies.Single();

            company.Name.Should().Be("Company A");
            company.Status.Should().Be(CompanyStatus.Pending);
        }

        [Fact]
        public async Task RegisterAsync_Should_Create_User()
        {
            var context = CreateContext();

            ApplicationUser? createdUser = null;

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, _) =>
                {
                    createdUser = u;
                })
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "test@test.dk",
                    Password = "Password123!",
                    CompanyName = "Company A"
                });

            createdUser.Should().NotBeNull();
            createdUser!.Email.Should().Be("test@test.dk");
        }

        [Fact]
        public async Task RegisterAsync_Should_Add_User_To_Role()
        {
            var context = CreateContext();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "test@test.dk",
                    Password = "Password123!",
                    CompanyName = "Company A"
                });

            userManager.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_When_Email_Already_Exists()
        {
            var context = CreateContext();

            var existingUser = CreateUser();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(
                    "test@test.dk"))
                .ReturnsAsync(existingUser);

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.RegisterAsync(
                    new RegisterDto
                    {
                        Email = "test@test.dk",
                        Password = "Password123!",
                        CompanyName = "Company A"
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("User with this email already exists.");
        }

        [Fact]
        public async Task RegisterAsync_Should_Validate_Unique_Company_Name()
        {
            var context = CreateContext();

            var validator = CreateUniquenessValidator();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager,
                validator: validator);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "test@test.dk",
                    Password = "Password123!",
                    CompanyName = "Company A"
                });

            validator.Verify(
                x => x.ValidateUniqueCompanyNameAsync(
                    "Company A",
                    null),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Trim_Company_Name()
        {
            var context = CreateContext();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "test@test.dk",
                    Password = "Password123!",
                    CompanyName = "  Company A  "
                });

            context.Companies
                .Single()
                .Name
                .Should()
                .Be("Company A");
        }

        [Fact]
        public async Task RegisterAsync_Should_Trim_Email()
        {
            var context = CreateContext();

            ApplicationUser? createdUser = null;

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync("test@test.dk"))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, _) =>
                {
                    createdUser = u;
                })
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "  test@test.dk  ",
                    Password = "Password123!",
                    CompanyName = "Company A"
                });

            createdUser!.Email.Should().Be("test@test.dk");

            userManager.Verify(
                x => x.FindByEmailAsync("test@test.dk"),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Publish_CompanyRegisteredEvent()
        {
            var context = CreateContext();

            var dispatcher = CreateEventDispatcher();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager,
                eventDispatcher: dispatcher);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "test@test.dk",
                    Password = "Password123!",
                    CompanyName = "Company A"
                });

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<CompanyRegisteredEvent>(e =>
                        e.CompanyName == "Company A" &&
                        e.UserEmail == "test@test.dk")),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Publish_UserRegisteredEvent()
        {
            var context = CreateContext();

            var dispatcher = CreateEventDispatcher();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService(
                context,
                userManager: userManager,
                eventDispatcher: dispatcher);

            await service.RegisterAsync(
                new RegisterDto
                {
                    Email = "test@test.dk",
                    Password = "Password123!",
                    CompanyName = "Company A"
                });

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.IsAny<UserRegisteredEvent>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Should_Rollback_When_User_Creation_Fails()
        {
            var context = CreateContext();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "User creation failed"
                        }));

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.RegisterAsync(
                    new RegisterDto
                    {
                        Email = "test@test.dk",
                        Password = "Password123!",
                        CompanyName = "Company A"
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("User creation failed");
        }

        [Fact]
        public async Task RegisterAsync_Should_Rollback_When_Role_Assignment_Fails()
        {
            var context = CreateContext();

            var userManager = CreateUserManager();

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            userManager
                .Setup(x => x.CreateAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<ApplicationUser>(),
                    "User"))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Role assignment failed"
                        }));

            var service = CreateService(
                context,
                userManager: userManager);

            Func<Task> act = async () =>
                await service.RegisterAsync(
                    new RegisterDto
                    {
                        Email = "test@test.dk",
                        Password = "Password123!",
                        CompanyName = "Company A"
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Role assignment failed");
        }
    }
}

