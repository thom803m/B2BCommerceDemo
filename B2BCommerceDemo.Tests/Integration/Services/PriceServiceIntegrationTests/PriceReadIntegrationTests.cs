using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.PriceServiceIntegrationTests
{
    public class PriceReadIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetPriceAsync_Should_Return_CompanyPrice_When_Exists()
        {
            var service = GetService<PriceService>();

            var company = await CreateCompanyAsync();

            var product = await CreateProductAsync();

            Context.CompanyPrices.Add(new CompanyPrice
            {
                CompanyId = company.Id,
                ProductId = product.Id,
                Price = 200m
            });

            await Context.SaveChangesAsync();

            var result = await service.GetPriceAsync(product.Id, company.Id);

            result.Should().Be(200m);
        }

        [Fact]
        public async Task GetPriceAsync_Should_Apply_PriceGroup_Adjustment_When_No_Override()
        {
            var service = GetService<PriceService>();

            var company = await CreateCompanyAsync();

            var product = await CreateProductAsync();

            var result = await service.GetPriceAsync(product.Id, company.Id);

            result.Should().Be(110m);
        }

        [Fact]
        public async Task GetPriceAsync_Should_Throw_When_Product_Not_Found()
        {
            var service = GetService<PriceService>();

            var company = await CreateCompanyAsync();

            var act = () => service.GetPriceAsync(999, company.Id);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task GetPricesForProductsAsync_Should_Return_All_Prices()
        {
            var service = GetService<PriceService>();

            var company = await CreateCompanyAsync();

            var p1 = await CreateProductAsync();
            var p2 = await CreateProductAsync();

            var result = await service.GetPricesForProductsAsync(
                new List<int> { p1.Id, p2.Id },
                company.Id);

            result.Should().HaveCount(2);
        }
    }
}

