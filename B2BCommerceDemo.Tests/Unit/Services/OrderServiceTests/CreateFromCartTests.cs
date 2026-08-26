using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests
{
    public class CreateFromCartTests : OrderServiceTestBase
    {
        [Fact]
        public async Task CreateFromCartAsync_Should_Create_Order_From_Cart()
        {
            var context = CreateContext();
            var priceService = new Mock<IPriceService>();

            context.Products.Add(CreateProduct(1, price: 100, stock: 10));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 2 }
                }
            });

            priceService
                .Setup(x => x.GetPricesForProductsAsync(
                    It.IsAny<List<int>>(), 1))
                .ReturnsAsync(new Dictionary<int, decimal>
                {
                    { 1, 100m }
                });

            await context.SaveChangesAsync();

            var service = CreateService(context, priceService: priceService);

            var result = await service.CreateFromCartAsync(
                1,
                "user1",
                "key-1");

            result.WasCreated.Should().BeTrue();
            result.Order.Should().NotBeNull();
            result.Order.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Deduct_Stock()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, price: 100, stock: 10));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 3 }
                }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.CreateFromCartAsync(1, "user1", "key-1");

            var product = context.Products.First();
            product.AvailableStock.Should().Be(7);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Empty_Cart_After_Order()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 1 }
                }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.CreateFromCartAsync(1, "user1", "key-1");

            context.CartItems.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Use_CompanyAccessValidator()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 1 }
                }
            });

            await context.SaveChangesAsync();

            var validator = CreateValidator();
            var service = CreateService(context, validator);

            await service.CreateFromCartAsync(1, "user1", "key-1");

            validator.Verify(
                x => x.ValidateCompanyActiveAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Throw_When_Cart_Is_Empty()
        {
            var context = CreateContext();

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>()
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.CreateFromCartAsync(1, "user1", "key-1");

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Cart is empty");
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Throw_When_Stock_Is_Insufficient()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, stock: 1));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 5 }
                }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.CreateFromCartAsync(1, "user1", "key-1");

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Not enough stock*");
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Return_Existing_Order_When_Idempotency_Key_Exists()
        {
            var context = CreateContext();

            var order = CreateOrder(
                id: 100,
                companyId: 1,
                userId: "user1");

            context.Orders.Add(order);

            context.IdempotencyRecords.Add(
                new IdempotencyRecord
                {
                    Key = "same-key",
                    CompanyId = 1,
                    UserId = "user1",
                    OrderId = 100,
                    CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.CreateFromCartAsync(
                1,
                "user1",
                "same-key");

            result.WasCreated.Should().BeFalse();
            result.Order.Id.Should().Be(100);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Not_Create_New_Order_When_Idempotency_Key_Exists()
        {
            var context = CreateContext();

            var order = CreateOrder(
                id: 100,
                companyId: 1,
                userId: "user1");

            context.Orders.Add(order);

            context.IdempotencyRecords.Add(
                new IdempotencyRecord
                {
                    Key = "same-key",
                    CompanyId = 1,
                    UserId = "user1",
                    OrderId = 100,
                    CreatedAt = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.CreateFromCartAsync(
                1,
                "user1",
                "same-key");

            context.Orders.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Throw_When_Idempotency_Record_Order_Is_Missing()
        {
            var context = CreateContext();

            context.IdempotencyRecords.Add(
                new IdempotencyRecord
                {
                    Key = "same-key",
                    CompanyId = 1,
                    UserId = "user1",
                    OrderId = 999
                });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.CreateFromCartAsync(
                    1,
                    "user1",
                    "same-key");

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Idempotency record exists but order missing");
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Use_Prices_From_PriceService()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1));

            context.Carts.Add(
                new Cart
                {
                    CompanyId = 1,
                    UserId = "user1",
                    Items =
                    [
                        new CartItem
                        {
                            ProductId = 1,
                            Quantity = 2
                        }
                    ]
                });

            await context.SaveChangesAsync();

            var priceService = new Mock<IPriceService>();

            priceService
                .Setup(x => x.GetPricesForProductsAsync(
                    It.IsAny<List<int>>(),
                    1))
                .ReturnsAsync(new Dictionary<int, decimal>
                {
                    { 1, 250m }
                });

            var service = CreateService(
                context,
                priceService: priceService);

            var result = await service.CreateFromCartAsync(
                1,
                "user1",
                "key-1");

            result.Order.Total.Should().Be(500m);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Publish_OrderCreated_Event()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1));

            context.Carts.Add(
                new Cart
                {
                    CompanyId = 1,
                    UserId = "user1",
                    Items =
                    [
                        new CartItem
                {
                    ProductId = 1,
                    Quantity = 1
                }
                    ]
                });

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var priceService = new Mock<IPriceService>();

            priceService
                .Setup(x => x.GetPricesForProductsAsync(
                    It.IsAny<List<int>>(),
                    1))
                .ReturnsAsync(new Dictionary<int, decimal>
                {
            { 1, 100m }
                });

            var service = CreateService(
                context,
                priceService: priceService,
                eventDispatcher: dispatcher);

            await service.CreateFromCartAsync(
                1,
                "user1",
                "key-1");

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.IsAny<OrderCreatedEvent>()),
                Times.Once);
        }
    }
}
