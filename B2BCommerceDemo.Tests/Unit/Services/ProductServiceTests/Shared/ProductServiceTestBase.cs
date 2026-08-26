using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests.Shared
{
    public abstract class ProductServiceTestBase
    {
        protected static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        protected static Product CreateProduct(
            int id,
            decimal basePrice,
            bool isActive = true)
        {
            return new Product
            {
                Id = id,
                Sku = $"SKU{id}",
                Name = $"Product {id}",
                BasePrice = basePrice,
                IsActive = isActive,
                Ean = $"EAN{id}",
                AvailableStock = 10,
                PurchasedQuantity = 0,
                RowVersion = BitConverter.GetBytes(id)
            };
        }

        protected static Brand CreateBrand(
            int id = 1,
            string name = "Brand")
        {
            return new Brand
            {
                Id = id,
                Name = name
            };
        }

        protected static Category CreateCategory(
            int id = 1,
            string name = "Category")
        {
            return new Category
            {
                Id = id,
                Name = name
            };
        }

        protected static Mock<IValidateUniqueness> CreateUniquenessValidator()
        {
            var validator = new Mock<IValidateUniqueness>();

            validator.Setup(x => x.ValidateUniqueSkuAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            validator.Setup(x => x.ValidateUniqueEanAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            validator.Setup(x => x.ValidateUniqueBrandNameAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            validator.Setup(x => x.ValidateUniqueCategoryNameAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            validator.Setup(x => x.ValidateUniqueCompanyNameAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            return validator;
        }

        protected static ProductService CreateService(
            AppDbContext context,
            Mock<IValidateUniqueness>? validate = null,
            Mock<IClock>? clock = null,
            Mock<IPriceService>? priceService = null)
        {
            validate ??= CreateUniquenessValidator();

            clock ??= new Mock<IClock>();

            clock.Setup(x => x.UtcNow)
                 .Returns(new DateTime(2025, 1, 1));

            if (priceService == null)
            {
                priceService = new Mock<IPriceService>();

                priceService
                    .Setup(x => x.GetPricesForProductsAsync(
                        It.IsAny<List<int>>(),
                        It.IsAny<int>()))
                    .ReturnsAsync(new Dictionary<int, decimal>
                    {
                        { 1, 100m }
                    });
            }

            return new ProductService(
                context,
                validate.Object,
                clock.Object,
                priceService.Object);
        }
    }
}
