using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductServiceIntegrationTests
{
    public class ProductUpdateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdateProduct_Should_Update_Product()
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

            var updated = await service.UpdateProductAsync(product.Id, new UpdateProductDto
            {
                Name = "iPhone Updated",
                Sku = "SKU1",
                Ean = "123",
                BasePrice = 1200,
                BrandId = brand.Id,
                CategoryId = category.Id
            });

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("iPhone Updated");
            updated.BasePrice.Should().Be(1200);

            var fromDb = await service.GetProductByIdAsync(product.Id, null, true);
            fromDb!.Name.Should().Be("iPhone Updated");
        }

        [Fact]
        public async Task UpdateProduct_Should_Throw_When_Product_Not_Found()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();
            var category = await CreateCategoryAsync();

            Func<Task> act = async () =>
                await service.UpdateProductAsync(999, new UpdateProductDto
                {
                    Name = "Test",
                    Sku = "SKU1",
                    Ean = "123",
                    BasePrice = 1000,
                    BrandId = brand.Id,
                    CategoryId = category.Id
                });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }
    }
}

