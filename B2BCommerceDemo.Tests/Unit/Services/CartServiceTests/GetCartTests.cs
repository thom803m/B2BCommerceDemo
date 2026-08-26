using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CartServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CartServiceTests
{
    public class GetCartTests : CartServiceTestBase
    {
        [Fact]
        public async Task GetCartAsync_Should_Return_Empty_Cart_When_Not_Found()
        {
            var context = CreateContext();

            var validator = CreateValidator();
            var priceService = CreatePriceService();

            var service = CreateService(context);

            var result = await service.GetCartAsync(
                1,
                "user1");

            result.Should().NotBeNull();
            result.CompanyId.Should().Be(1);
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCartAsync_Should_Return_Empty_Cart_When_No_Items()
        {
            var context = CreateContext();

            context.Carts.Add(
                CreateCart(1, "user1"));

            await context.SaveChangesAsync();

            var validator = CreateValidator();
            var priceService = CreatePriceService();

            var service = CreateService(context);

            var result = await service.GetCartAsync(
                1,
                "user1");

            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCartAsync_Should_Return_Cart_With_Items()
        {
            var context = CreateContext();

            var product = CreateProduct(1);

            context.Products.Add(product);

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

            var validator = CreateValidator();
            var priceService = CreatePriceService();

            var service = CreateService(context);

            var result = await service.GetCartAsync(
                1,
                "user1");

            result.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCartAsync_Should_Use_Current_Prices_From_PriceService()
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
                        Quantity = 1,
                        UnitPrice = 50
                    }
                }
            });

            await context.SaveChangesAsync();

            var validator = CreateValidator();

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

            await service.GetCartAsync(
                1,
                "user1");

            priceService.Verify(
                x => x.GetPricesForProductsAsync(
                    It.Is<List<int>>(l => l.Contains(1)),
                    1),
                Times.Once);
        }

        [Fact]
        public async Task GetCartAsync_Should_Validate_Company_Is_Active()
        {
            var context = CreateContext();

            var validator = CreateValidator();
            var priceService = CreatePriceService();

            var service = CreateService(
                context,
                validator: validator);

            await service.GetCartAsync(
                1,
                "user1");

            validator.Verify(
                x => x.ValidateCompanyActiveAsync(1),
                Times.Once);
        }
    }
}
