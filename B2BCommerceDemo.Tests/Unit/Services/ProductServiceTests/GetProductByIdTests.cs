using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests
{
    public class GetProductByIdTests : ProductServiceTestBase
    {
        [Fact]
        public async Task GetProductByIdAsync_Should_Return_Product_When_Exists()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetProductByIdAsync(
                id: 1,
                companyId: null,
                isAdmin: true);

            result.Should().NotBeNull();
            result!.BasePrice.Should().Be(100);
            result!.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Throw_When_Product_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.GetProductByIdAsync(999, null, true);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Throw_When_Inactive_And_Not_Admin()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100, isActive: false));
            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.GetProductByIdAsync(1, null, false);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Use_CompanyPrice_When_Available()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));
            await context.SaveChangesAsync();

            var priceService = new Mock<IPriceService>();

            priceService
                .Setup(x => x.GetPriceAsync(1, 1))
                .ReturnsAsync(75);

            var service = CreateService(
                context,
                priceService: priceService);

            var result = await service.GetProductByIdAsync(
                1, 1, false);

            result!.BasePrice.Should().Be(75);
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Fallback_To_BasePrice_When_No_CompanyId()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetProductByIdAsync(
                1, null, true);

            result!.BasePrice.Should().Be(100);
        }
    }
}

