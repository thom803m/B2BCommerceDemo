using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductServiceIntegrationTests
{
    public class UpdateProductContentIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdateProductContentAsync_Should_Update_Content_Fields()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync();

            var product = await Context.Products.SingleAsync();

            var dto = new UpdateProductContentDto
            {
                Description = "Manual product description",
                SpecificationsJson = "{\"dpi\":\"20000\",\"connection\":\"wired\"}",
                ContentLocked = true
            };

            var result = await service.UpdateProductContentAsync(product.Id, dto);

            result.Should().NotBeNull();
            result!.Description.Should().Be("Manual product description");
            result.SpecificationsJson.Should().Be("{\"dpi\":\"20000\",\"connection\":\"wired\"}");
            result.ContentSource.Should().Be("Manual");
            result.ContentLocked.Should().BeTrue();

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.Description.Should().Be("Manual product description");
            updated.SpecificationsJson.Should().Be("{\"dpi\":\"20000\",\"connection\":\"wired\"}");
            updated.ContentSource.Should().Be("Manual");
            updated.ContentLocked.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProductContentAsync_Should_Not_Update_Commercial_Fields()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(
                sku: "SKU001",
                name: "Original Product",
                basePrice: 100);

            var product = await Context.Products.SingleAsync();

            var originalSku = product.Sku;
            var originalName = product.Name;
            var originalBasePrice = product.BasePrice;
            var originalEan = product.Ean;
            var originalAvailableStock = product.AvailableStock;

            var dto = new UpdateProductContentDto
            {
                Description = "New text",
                SpecificationsJson = "{\"color\":\"black\"}",
                ContentLocked = true
            };

            await service.UpdateProductContentAsync(product.Id, dto);

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.Sku.Should().Be(originalSku);
            updated.Name.Should().Be(originalName);
            updated.BasePrice.Should().Be(originalBasePrice);
            updated.Ean.Should().Be(originalEan);
            updated.AvailableStock.Should().Be(originalAvailableStock);
        }

        [Fact]
        public async Task UpdateProductContentAsync_Should_Throw_When_Product_Does_Not_Exist()
        {
            var service = GetService<ProductService>();

            var dto = new UpdateProductContentDto
            {
                Description = "Text"
            };

            await FluentActions
                .Invoking(() => service.UpdateProductContentAsync(999999, dto))
                .Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task UpdateProductContentAsync_Should_Set_ContentSource_To_Manual_When_Only_Specifications_Are_Updated()
        {
            var service = GetService<ProductService>();

            var product = await CreateProductAsync(
                sku: "SKU001",
                ean: "1234567890123");

            var dto = new UpdateProductContentDto
            {
                SpecificationsJson = "{\"color\":\"black\"}",
                ContentLocked = true
            };

            await service.UpdateProductContentAsync(product.Id, dto);

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.ContentSource.Should().Be("Manual");
            updated.SpecificationsJson.Should().Be("{\"color\":\"black\"}");
            updated.ContentLocked.Should().BeTrue();
        }
    }
}
