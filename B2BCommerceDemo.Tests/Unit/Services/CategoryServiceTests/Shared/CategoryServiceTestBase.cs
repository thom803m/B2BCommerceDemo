using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests.Shared
{
    public abstract class CategoryServiceTestBase
    {
        protected static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        protected static Category CreateCategory(int id = 1, string name = "Category")
        {
            return new Category
            {
                Id = id,
                Name = name
            };
        }

        protected static Mock<IValidateUniqueness> CreateValidator()
        {
            var mock = new Mock<IValidateUniqueness>();

            mock.Setup(x => x.ValidateUniqueCategoryNameAsync(
                It.IsAny<string>(),
                It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        protected static CategoryService CreateService(
            AppDbContext context,
            Mock<IValidateUniqueness>? validator = null)
        {
            validator ??= CreateValidator();

            return new CategoryService(context, validator.Object);
        }
    }
}
