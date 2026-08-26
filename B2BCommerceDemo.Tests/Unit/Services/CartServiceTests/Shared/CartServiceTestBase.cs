using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CartServiceTests.Shared
{
    public abstract class CartServiceTestBase
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
            int stock = 10,
            bool active = true)
        {
            return new Product
            {
                Id = id,
                Name = $"Product {id}",
                Sku = $"SKU{id}",
                BasePrice = 100,
                AvailableStock = stock,
                IsActive = active,
                Ean = $"EAN{id}",
                RowVersion = BitConverter.GetBytes(id)
            };
        }

        protected static Cart CreateCart(
            int companyId,
            string userId)
        {
            return new Cart
            {
                CompanyId = companyId,
                UserId = userId,
                Items = new List<CartItem>()
            };
        }

        protected static Mock<ICompanyAccessValidator> CreateValidator()
        {
            var validator = new Mock<ICompanyAccessValidator>();

            validator
                .Setup(x => x.ValidateCompanyActiveAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            return validator;
        }

        protected static CartService CreateService(
            AppDbContext context,
            Mock<ICompanyAccessValidator>? validator = null,
            Mock<IPriceService>? priceService = null)
        {
            validator ??= CreateValidator();
            priceService ??= CreatePriceService();

            return new CartService(
                context,
                validator.Object,
                priceService.Object);
        }

        protected static Mock<IPriceService> CreatePriceService()
        {
            var service = new Mock<IPriceService>();

            service
                .Setup(x => x.GetPriceAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(100m);

            service
                .Setup(x => x.GetPricesForProductsAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new Dictionary<int, decimal>
                {
                { 1, 100m }
                });

            return service;
        }
    }
}

