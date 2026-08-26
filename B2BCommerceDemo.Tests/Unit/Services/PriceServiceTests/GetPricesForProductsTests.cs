using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.PriceServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.PriceServiceTests
{
    public class GetPricesForProductsTests : PriceServiceTestBase
    {
        [Fact]
        public async Task GetPricesForProductsAsync_Should_Return_Override_Prices()
        {
            var context = CreateContext();

            context.Products.AddRange(
                CreateProduct(1, 100),
                CreateProduct(2, 200));

            context.CompanyPrices.AddRange(
                new CompanyPrice
                {
                    ProductId = 1,
                    CompanyId = 1,
                    Price = 75
                },
                new CompanyPrice
                {
                    ProductId = 2,
                    CompanyId = 1,
                    Price = 150
                });

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            var service = CreateService(context, validator);

            var result = await service.GetPricesForProductsAsync(
                [1, 2],
                1);

            result[1].Should().Be(75);
            result[2].Should().Be(150);
        }

        [Fact]
        public async Task GetPricesForProductsAsync_Should_Apply_PriceGroup_Adjustment()
        {
            var context = CreateContext();

            context.Products.AddRange(
                CreateProduct(1, 100),
                CreateProduct(2, 200));

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            validator.Setup(x => x.GetActiveCompanyAsync(1))
                .ReturnsAsync(new Company
                {
                    Id = 1,
                    PriceGroup = new PriceGroup
                    {
                        PercentageAdjustment = 10
                    }
                });

            var service = CreateService(context, validator);

            var result = await service.GetPricesForProductsAsync(
                [1, 2],
                1);

            result[1].Should().Be(110);
            result[2].Should().Be(220);
        }

        [Fact]
        public async Task GetPricesForProductsAsync_Should_Return_Empty_When_No_Products_Exist()
        {
            var context = CreateContext();

            var validator = CreateValidator();

            validator.Setup(x => x.GetActiveCompanyAsync(1))
                .ReturnsAsync(new Company
                {
                    Id = 1
                });

            var service = CreateService(context, validator);

            var result = await service.GetPricesForProductsAsync(
                [999],
                1);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPricesForProductsAsync_Should_Mix_Override_And_Adjusted_Prices()
        {
            var context = CreateContext();

            context.Products.AddRange(
                CreateProduct(1, 100),
                CreateProduct(2, 200));

            context.CompanyPrices.Add(
                new CompanyPrice
                {
                    ProductId = 1,
                    CompanyId = 1,
                    Price = 80
                });

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            validator.Setup(x => x.GetActiveCompanyAsync(1))
                .ReturnsAsync(new Company
                {
                    Id = 1,
                    PriceGroup = new PriceGroup
                    {
                        PercentageAdjustment = 10
                    }
                });

            var service = CreateService(context, validator);

            var result = await service.GetPricesForProductsAsync(
                [1, 2],
                1);

            result[1].Should().Be(80);
            result[2].Should().Be(220);
        }
    }
}
