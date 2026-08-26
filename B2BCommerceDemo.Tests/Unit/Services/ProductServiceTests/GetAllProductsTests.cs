using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests
{
    public class GetAllProductsTests : ProductServiceTestBase 
    {
        [Fact]
        public async Task GetAllProductsAsync_AsAdmin_Should_Return_All_Products_With_BasePrice()
        {
            var context = CreateContext();

            context.Products.AddRange(
                CreateProduct(1, 100),
                CreateProduct(2, 200));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetAllProductsAsync(
                companyId: null,
                isAdmin: true);

            result.Should().HaveCount(2);
            result[0].BasePrice.Should().Be(100);
            result[1].BasePrice.Should().Be(200);
        }

        [Fact]
        public async Task GetAllProductsAsync_AsNonAdmin_Should_Filter_Inactive_Products()
        {
            var context = CreateContext();

            context.Products.AddRange(
                CreateProduct(1, 100, isActive: true),
                CreateProduct(2, 200, isActive: false));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetAllProductsAsync(
                companyId: null,
                isAdmin: false);

            result.Should().HaveCount(1);
            result[0].BasePrice.Should().Be(100);
        }

        [Fact]
        public async Task GetAllProductsAsync_Should_Use_Company_Prices_When_Available()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));

            await context.SaveChangesAsync();

            var priceService = new Mock<IPriceService>();

            priceService
                .Setup(x => x.GetPricesForProductsAsync(
                    It.IsAny<List<int>>(),
                    1))
                .ReturnsAsync(new Dictionary<int, decimal>
                {
                    [1] = 75
                });

            var service = CreateService(
                context,
                priceService: priceService);

            var result = await service.GetAllProductsAsync(
                companyId: 1,
                isAdmin: false);

            result.Should().HaveCount(1);
            result[0].BasePrice.Should().Be(75);
        }
    }
}

