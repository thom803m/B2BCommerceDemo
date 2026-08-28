using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared
{
    public abstract class CompanyServiceTestBase
    {
        protected static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        protected static Company CreateCompany(
            int id,
            string name,
            CompanyStatus status = CompanyStatus.Active)
        {
            return new Company
            {
                Id = id,
                Name = name,
                Status = status
            };
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

        protected static CompanyService CreateService(
            AppDbContext context,
            Mock<IValidateUniqueness>? validate = null,
            Mock<IEventDispatcher>? eventDispatcher = null,
            Mock<UserManager<ApplicationUser>>? userManager = null)
        {
            validate ??= CreateUniquenessValidator();
            eventDispatcher ??= new Mock<IEventDispatcher>();
            userManager ??= CreateUserManagerMock();

            return new CompanyService(
                context,
                eventDispatcher.Object,
                validate.Object,
                userManager.Object);
        }

        protected static PriceGroup CreatePriceGroup(int id)
        {
            return new PriceGroup
            {
                Id = id,
                Name = $"PriceGroup {id}"
            };
        }

        protected static ApproveCompanyDto CreateApproveCompanyDto(
            int priceGroupId,
            string rackbeatCustomerNumber = "900000580")
        {
            return new ApproveCompanyDto
            {
                PriceGroupId = priceGroupId,
                RackbeatCustomerNumber = rackbeatCustomerNumber
            };
        }

        protected static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();

            var userManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

            userManager
                .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            return userManager;
        }
    }
}
