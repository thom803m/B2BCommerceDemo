using B2BCommerceDemo.Core.DTOs.Carts;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CartServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CartServiceTests
{
    public class AddItemTests : CartServiceTestBase
    {
        [Fact]
        public async Task AddItemAsync_Should_Create_Cart_When_Not_Exists()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.AddItemAsync(
                1,
                "user1",
                new CreateCartItemDto
                {
                    ProductId = 1,
                    Quantity = 1
                });

            context.Carts.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddItemAsync_Should_Add_New_Item()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            context.Carts.Add(
                CreateCart(1, "user1"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.AddItemAsync(
                1,
                "user1",
                new CreateCartItemDto
                {
                    ProductId = 1,
                    Quantity = 2
                });

            result.Items.Should().HaveCount(1);
            result.Items[0].Quantity.Should().Be(2);
        }

        [Fact]
        public async Task AddItemAsync_Should_Increase_Quantity_For_Existing_Item()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 100
                    }
                }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.AddItemAsync(
                1,
                "user1",
                new CreateCartItemDto
                {
                    ProductId = 1,
                    Quantity = 3
                });

            result.Items.Single().Quantity.Should().Be(5);
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Product_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.AddItemAsync(
                    1,
                    "user1",
                    new CreateCartItemDto
                    {
                        ProductId = 999,
                        Quantity = 1
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Product_Is_Inactive()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1, active: false));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.AddItemAsync(
                    1,
                    "user1",
                    new CreateCartItemDto
                    {
                        ProductId = 1,
                        Quantity = 1
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Product unavailable");
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Quantity_Is_Zero()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.AddItemAsync(
                    1,
                    "user1",
                    new CreateCartItemDto
                    {
                        ProductId = 1,
                        Quantity = 0
                    });

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than 0");
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Stock_Is_Insufficient()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1, stock: 2));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.AddItemAsync(
                    1,
                    "user1",
                    new CreateCartItemDto
                    {
                        ProductId = 1,
                        Quantity = 5
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Not enough stock");
        }

        [Fact]
        public async Task AddItemAsync_Should_Call_PriceService_For_New_Item()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            await context.SaveChangesAsync();

            var priceService = CreatePriceService();

            var service = CreateService(
                context,
                priceService: priceService);

            await service.AddItemAsync(
                1,
                "user1",
                new CreateCartItemDto
                {
                    ProductId = 1,
                    Quantity = 1
                });

            priceService.Verify(
                x => x.GetPriceAsync(1, 1),
                Times.Once);
        }

        [Fact]
        public async Task AddItemAsync_Should_Validate_Company_Is_Active()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            var service = CreateService(
                context,
                validator: validator);

            await service.AddItemAsync(
                1,
                "user1",
                new CreateCartItemDto
                {
                    ProductId = 1,
                    Quantity = 1
                });

            validator.Verify(
                x => x.ValidateCompanyActiveAsync(1),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Existing_Quantity_Exceeds_Stock()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1, stock: 5));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 4,
                        UnitPrice = 100
                    }
                }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.AddItemAsync(
                    1,
                    "user1",
                    new CreateCartItemDto
                    {
                        ProductId = 1,
                        Quantity = 2
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Not enough stock");
        }

        [Fact]
        public async Task AddItemAsync_Should_Not_Call_PriceService_For_Existing_Item()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 100
                    }
                }
            });

            await context.SaveChangesAsync();

            var priceService = new Mock<IPriceService>();

            priceService
                .Setup(x => x.GetPricesForProductsAsync(
                    It.IsAny<List<int>>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new Dictionary<int, decimal>
                {
                    { 1, 100m }
                });

            var service = CreateService(
                context,
                priceService: priceService);

            await service.AddItemAsync(
                1,
                "user1",
                new CreateCartItemDto
                {
                    ProductId = 1,
                    Quantity = 1
                });

            priceService.Verify(
                x => x.GetPriceAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
                Times.Never);
        }
    }
}

