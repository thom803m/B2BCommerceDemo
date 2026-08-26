using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests.Shared
{
    public abstract class OrderServiceTestBase
    {
        protected static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppDbContext(options);
        }

        protected static Product CreateProduct(int id, decimal price = 100, int stock = 10)
        {
            return new Product
            {
                Id = id,
                Sku = $"SKU{id}",
                Name = $"Product {id}",
                BasePrice = price,
                AvailableStock = stock,
                IsActive = true,
                Ean = $"EAN{id}",
                RowVersion = BitConverter.GetBytes(id)
            };
        }

        protected static Order CreateOrder(int id, int companyId, string userId)
        {
            return new Order
            {
                Id = id,
                CompanyId = companyId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Total = 100,
                Items = new List<OrderItem>()
            };
        }

        protected static Cart CreateCart(int companyId, string userId)
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

        protected static OrderService CreateService(
            AppDbContext context,
            Mock<ICompanyAccessValidator>? validator = null,
            Mock<IPriceService>? priceService = null,
            Mock<IEventDispatcher>? eventDispatcher = null,
            Mock<IClock>? clock = null)
        {
            validator ??= CreateValidator();
            eventDispatcher ??= new Mock<IEventDispatcher>();
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

            return new OrderService(
                context,
                CreateUserManager(),
                eventDispatcher.Object,
                validator.Object,
                clock.Object,
                priceService.Object);
        }

        protected static UserManager<ApplicationUser> CreateUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new UserManager<ApplicationUser>(
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
    }
}
