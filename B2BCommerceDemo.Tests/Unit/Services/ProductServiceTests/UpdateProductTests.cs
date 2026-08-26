using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests
{
    public class UpdateProductTests : ProductServiceTestBase
    {
        [Fact]
        public async Task UpdateProductAsync_Should_Update_Product_Correctly()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand(1, "Apple"));
            context.Brands.Add(CreateBrand(2, "Samsung"));

            context.Categories.Add(CreateCategory(1, "Phones"));
            context.Categories.Add(CreateCategory(2, "Tablets"));

            context.Products.Add(CreateProduct(1, 100));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateProductDto
            {
                Sku = " newsku ",
                Name = " New Product ",
                BasePrice = 250,
                Ean = "999999",
                BrandId = 2,
                CategoryId = 2
            };

            var result = await service.UpdateProductAsync(1, dto);

            result.Should().NotBeNull();
            result!.Sku.Should().Be("NEWSKU");
            result.Name.Should().Be("New Product");
            result.BasePrice.Should().Be(250);
        }

        [Fact]
        public async Task UpdateProductAsync_Should_Throw_When_Product_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var dto = new UpdateProductDto
            {
                Sku = "SKU1",
                Name = "Product",
                BasePrice = 100,
                BrandId = 1,
                CategoryId = 1
            };

            Func<Task> act =
                async () => await service.UpdateProductAsync(999, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task UpdateProductAsync_Should_Throw_When_Brand_Not_Found()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));
            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateProductDto
            {
                Sku = "SKU1",
                Name = "Product",
                BasePrice = 100,
                BrandId = 999,
                CategoryId = 1
            };

            Func<Task> act =
                async () => await service.UpdateProductAsync(1, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Brand not found");
        }

        [Fact]
        public async Task UpdateProductAsync_Should_Throw_When_Category_Not_Found()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));
            context.Brands.Add(CreateBrand());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateProductDto
            {
                Sku = "SKU1",
                Name = "Product",
                BasePrice = 100,
                BrandId = 1,
                CategoryId = 999
            };

            Func<Task> act =
                async () => await service.UpdateProductAsync(1, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Category not found");
        }

        [Fact]
        public async Task UpdateProductAsync_Should_Call_Validation()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());
            context.Categories.Add(CreateCategory());

            context.Products.Add(CreateProduct(1, 100));

            await context.SaveChangesAsync();

            var validate = new Mock<IValidateUniqueness>();

            validate.Setup(x => x.ValidateUniqueSkuAsync("SKU1", 1))
                .Returns(Task.CompletedTask);

            validate.Setup(x => x.ValidateUniqueEanAsync("EAN1", 1))
                .Returns(Task.CompletedTask);

            var dto = new UpdateProductDto
            {
                Sku = "SKU1",
                Ean = "EAN1",
                Name = "iPhone",
                BasePrice = 100,
                BrandId = 1,
                CategoryId = 1
            };

            var service = CreateService(
                context, 
                validate);

            await service.UpdateProductAsync(1, dto);

            validate.Verify(x => x.ValidateUniqueSkuAsync("SKU1", 1), Times.Once);
            validate.Verify(x => x.ValidateUniqueEanAsync("EAN1", 1), Times.Once);
        }
    }
}

