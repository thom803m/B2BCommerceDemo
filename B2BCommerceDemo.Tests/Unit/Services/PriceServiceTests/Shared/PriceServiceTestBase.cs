using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.PriceServiceTests.Shared
{
    public abstract class PriceServiceTestBase
    {
        protected static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        protected static Product CreateProduct(int id, decimal basePrice)
        {
            return new Product
            {
                Id = id,
                Sku = $"SKU{id}",
                Name = $"Product {id}",
                BasePrice = basePrice,
                Ean = $"EAN{id}",
                IsActive = true,
                RowVersion = new byte[] { 1 }
            };
        }

        protected static Mock<ICompanyAccessValidator> CreateValidator()
        {
            var validator = new Mock<ICompanyAccessValidator>();

            validator.Setup(x => x.GetActiveCompanyAsync(It.IsAny<int>()))
                .ReturnsAsync(new Company
                {
                    Id = 1,
                    PriceGroup = null
                });

            return validator;
        }

        protected static PriceService CreateService(
            AppDbContext context,
            Mock<ICompanyAccessValidator>? validator = null)
        {
            validator ??= CreateValidator();

            return new PriceService(
                context,
                validator.Object);
        }

        protected static PriceService CreateService(AppDbContext context)
        {
            return CreateService(context, null);
        }
    }
}
