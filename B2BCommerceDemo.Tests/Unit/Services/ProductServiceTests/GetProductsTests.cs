using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests
{
    public class GetProductsTests : ProductServiceTestBase
    {
        [Fact]
        public async Task GetProductsAsync_Should_Apply_Paging()
        {
            var context = CreateContext();

            for (int i = 1; i <= 5; i++)
            {
                context.Products.Add(CreateProduct(i, 100));
            }

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    Page = 1,
                    PageSize = 2
                },
                companyId: null,
                isAdmin: true);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(5);
        }

        [Fact]
        public async Task GetProductsAsync_Should_Filter_By_Search()
        {
            var context = CreateContext();

            var p1 = CreateProduct(1, 100);
            var p2 = CreateProduct(2, 100);

            p1.Name = "iPhone";
            p2.Name = "Samsung";

            context.Products.AddRange(p1, p2);

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    Search = "iPhone"
                },
                companyId: null,
                isAdmin: true);

            result.Items.Should().HaveCount(1);
            result.Items[0].Name.Should().Be("iPhone");
        }
    }
}

