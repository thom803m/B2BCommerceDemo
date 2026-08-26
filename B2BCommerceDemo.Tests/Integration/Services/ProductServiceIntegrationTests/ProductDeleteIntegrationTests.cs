using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductServiceIntegrationTests
{
    public class ProductDeleteIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task DeleteProduct_Should_Remove_Product()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();
            var category = await CreateCategoryAsync();

            var product = new Product
            {
                Name = "iPhone",
                Sku = "SKU1",
                BasePrice = 1000,
                IsActive = true,
                BrandId = brand.Id,
                CategoryId = category.Id,
                RowVersion = new byte[8]
            };

            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            await service.DeleteProductAsync(product.Id);

            var exists = await Context.Products.FindAsync(product.Id);

            exists.Should().BeNull();
        }

        [Fact]
        public async Task DeleteProduct_Should_Throw_When_Not_Found()
        {
            var service = GetService<ProductService>();

            Func<Task> act = async () =>
                await service.DeleteProductAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }
    }
}

