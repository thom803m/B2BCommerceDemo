using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.PriceServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.PriceServiceTests
{
    public class GetPriceTests : PriceServiceTestBase
    {
        [Fact]
        public async Task GetPriceAsync_Should_Return_CompanyPrice_When_Override_Exists()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));

            context.CompanyPrices.Add(new CompanyPrice
            {
                ProductId = 1,
                CompanyId = 1,
                Price = 75
            });

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            var service = CreateService(context, validator);

            var result = await service.GetPriceAsync(1, 1);

            result.Should().Be(75);
        }

        [Fact]
        public async Task GetPriceAsync_Should_Return_BasePrice_With_PriceGroup_Adjustment()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));

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

            var result = await service.GetPriceAsync(1, 1);

            result.Should().Be(110);
        }

        [Fact]
        public async Task GetPriceAsync_Should_Return_BasePrice_When_Adjustment_Is_Zero()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            validator.Setup(x => x.GetActiveCompanyAsync(1))
                .ReturnsAsync(new Company
                {
                    Id = 1,
                    PriceGroup = new PriceGroup
                    {
                        PercentageAdjustment = 0
                    }
                });

            var service = CreateService(context, validator);

            var result = await service.GetPriceAsync(1, 1);

            result.Should().Be(100);
        }

        [Fact]
        public async Task GetPriceAsync_Should_Round_To_Two_Decimals()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 99.99m));

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            validator.Setup(x => x.GetActiveCompanyAsync(1))
                .ReturnsAsync(new Company
                {
                    PriceGroup = new PriceGroup
                    {
                        PercentageAdjustment = 12.5m
                    }
                });

            var service = CreateService(context, validator);

            var result = await service.GetPriceAsync(1, 1);

            result.Should().Be(112.49m);
        }

        [Fact]
        public async Task GetPriceAsync_Should_Return_BasePrice_When_No_PriceGroup()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));

            await context.SaveChangesAsync();

            var validator = CreateValidator();

            validator.Setup(x => x.GetActiveCompanyAsync(1))
                .ReturnsAsync(new Company
                {
                    Id = 1,
                    PriceGroup = null
                });

            var service = CreateService(context, validator);

            var result = await service.GetPriceAsync(1, 1);

            result.Should().Be(100);
        }

        [Fact]
        public async Task GetPriceAsync_Should_Throw_When_Product_Does_Not_Exist()
        {
            var context = CreateContext();

            var validator = CreateValidator();

            validator.Setup(x => x.GetActiveCompanyAsync(1))
                .ReturnsAsync(new Company
                {
                    Id = 1
                });

            var service = CreateService(context, validator);

            Func<Task> act =
                async () => await service.GetPriceAsync(999, 1);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }
    }
} 
