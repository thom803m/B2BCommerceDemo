using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.AuthServiceTests.Shared
{
    public abstract class AuthServiceTestBase
    {
        protected static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x =>
                    x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppDbContext(options);
        }

        protected static Mock<UserManager<ApplicationUser>> CreateUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        protected static Mock<IEventDispatcher> CreateEventDispatcher()
        {
            return new Mock<IEventDispatcher>();
        }

        protected static Mock<IValidateUniqueness> CreateUniquenessValidator()
        {
            var validator = new Mock<IValidateUniqueness>();

            validator
                .Setup(x => x.ValidateUniqueCompanyNameAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            return validator;
        }

        protected static Mock<IJwtService> CreateJwtService()
        {
            var jwt = new Mock<IJwtService>();

            jwt.Setup(x => x.GenerateToken(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<int?>()))
               .ReturnsAsync("jwt-token");

            return jwt;
        }

        protected static AuthService CreateService(
            AppDbContext context,
            Mock<UserManager<ApplicationUser>>? userManager = null,
            Mock<IEventDispatcher>? eventDispatcher = null,
            Mock<IValidateUniqueness>? validator = null,
            Mock<IJwtService>? jwtService = null)
        {
            userManager ??= CreateUserManager();
            eventDispatcher ??= CreateEventDispatcher();
            validator ??= CreateUniquenessValidator();
            jwtService ??= CreateJwtService();

            return new AuthService(
                context,
                userManager.Object,
                eventDispatcher.Object,
                validator.Object,
                jwtService.Object);
        }

        protected static ApplicationUser CreateUser(
            string id = "user1",
            string email = "test@test.dk",
            int? companyId = 1,
            bool emailConfirmed = true)
        {
            return new ApplicationUser
            {
                Id = id,
                Email = email,
                UserName = email,
                CompanyId = companyId,
                EmailConfirmed = emailConfirmed
            };
        }

        protected static Company CreateCompany(
            int id,
            string name = "Company A",
            CompanyStatus status = CompanyStatus.Active)
        {
            return new Company
            {
                Id = id,
                Name = name,
                Status = status
            };
        }
    }
}
