using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests
{
    public class CreateProductTests : ProductServiceTestBase
    {
        [Fact]
        public async Task CreateProductAsync_Should_Create_Product_Correctly()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());
            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var clock = new Mock<IClock>();
            clock.Setup(x => x.UtcNow).Returns(new DateTime(2025, 1, 1));

            var service = CreateService(
                context, 
                clock: clock);

            var dto = new CreateProductDto
            {
                Sku = " sku123 ",
                Name = " iPhone ",
                BasePrice = 100,
                AvailableStock = 10,
                Ean = "123456",
                BrandId = 1,
                CategoryId = 1
            };

            var result = await service.CreateProductAsync(dto);

            result.Should().NotBeNull();
            result!.Sku.Should().Be("SKU123");
            result.Name.Should().Be("iPhone");
            result.BasePrice.Should().Be(100);
        }

        [Fact]
        public async Task CreateProductAsync_Should_Throw_When_Brand_Not_Found()
        {
            var context = CreateContext();

            context.Categories.Add(CreateCategory());
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new CreateProductDto
            {
                Sku = "SKU1",
                Name = "iPhone",
                Ean = "123456",
                BasePrice = 100,
                AvailableStock = 10,
                BrandId = 999,
                CategoryId = 1
            };

            Func<Task> act = async () => await service.CreateProductAsync(dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Brand not found");
        }

        [Fact]
        public async Task CreateProductAsync_Should_Throw_When_Category_Not_Found()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new CreateProductDto
            {
                Sku = "SKU1",
                Name = "iPhone",
                Ean = "123456",
                BasePrice = 100,
                AvailableStock = 10,
                BrandId = 1,
                CategoryId = 999
            };

            Func<Task> act = async () => await service.CreateProductAsync(dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Category not found");
        }

        [Fact]
        public async Task CreateProductAsync_Should_Call_Validation()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());
            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var validate = new Mock<IValidateUniqueness>();

            validate.Setup(x => x.ValidateUniqueSkuAsync("SKU1", null))
                .Returns(Task.CompletedTask);

            validate.Setup(x => x.ValidateUniqueEanAsync("EAN1", null))
                .Returns(Task.CompletedTask);

            var dto = new CreateProductDto
            {
                Sku = "SKU1",
                Ean = "EAN1",
                Name = "iPhone",
                BasePrice = 100,
                AvailableStock = 10,
                BrandId = 1,
                CategoryId = 1
            };

            var service = CreateService(
                context, 
                validate);

            await service.CreateProductAsync(dto);

            validate.Verify(x => x.ValidateUniqueSkuAsync("SKU1", null), Times.Once);
            validate.Verify(x => x.ValidateUniqueEanAsync("EAN1", null), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_Should_Set_IsActive_Based_On_Stock()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());
            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new CreateProductDto
            {
                Sku = "SKU1",
                Ean = "EAN1",
                Name = "Product",
                BasePrice = 100,
                AvailableStock = 0,
                BrandId = 1,
                CategoryId = 1
            };

            var result = await service.CreateProductAsync(dto);

            result!.IsActive.Should().BeFalse();
        }
    }
}

