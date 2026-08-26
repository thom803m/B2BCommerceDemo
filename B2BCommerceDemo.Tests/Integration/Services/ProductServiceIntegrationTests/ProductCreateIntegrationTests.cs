using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductServiceIntegrationTests
{
    public class ProductCreateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateProduct_Should_Create_Product()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();
            var category = await CreateCategoryAsync();

            var created = await service.CreateProductAsync(
                new CreateProductDto
                {
                    Name = "iPhone",
                    Sku = "SKU1",
                    Ean = "123",
                    BasePrice = 1000,
                    AvailableStock = 5,
                    BrandId = brand.Id,
                    CategoryId = category.Id
                });

            created.Should().NotBeNull();
            created!.Name.Should().Be("iPhone");
        }

        [Fact]
        public async Task CreateProduct_Should_Trim_And_Uppercase_Sku()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();
            var category = await CreateCategoryAsync();

            var created = await service.CreateProductAsync(
                new CreateProductDto
                {
                    Name = "iPhone",
                    Sku = " sku-1 ",
                    Ean = "123",
                    BasePrice = 1000,
                    AvailableStock = 5,
                    BrandId = brand.Id,
                    CategoryId = category.Id
                });

            created!.Sku.Should().Be("SKU-1");
        }

        [Fact]
        public async Task CreateProduct_Should_Throw_When_Brand_Not_Found()
        {
            var service = GetService<ProductService>();

            var category = await CreateCategoryAsync();

            Func<Task> act = async () =>
                await service.CreateProductAsync(
                    new CreateProductDto
                    {
                        Name = "iPhone",
                        Sku = "SKU1",
                        Ean = "123",
                        BasePrice = 1000,
                        AvailableStock = 5,
                        BrandId = 999,
                        CategoryId = category.Id
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Brand not found");
        }

        [Fact]
        public async Task CreateProduct_Should_Throw_When_Category_Not_Found()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();

            Func<Task> act = async () =>
                await service.CreateProductAsync(
                    new CreateProductDto
                    {
                        Name = "iPhone",
                        Sku = "SKU1",
                        Ean = "123",
                        BasePrice = 1000,
                        AvailableStock = 5,
                        BrandId = brand.Id,
                        CategoryId = 999
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Category not found");
        }

        [Fact]
        public async Task CreateProduct_Should_Throw_When_Sku_Already_Exists()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();
            var category = await CreateCategoryAsync();

            await service.CreateProductAsync(
                new CreateProductDto
                {
                    Name = "iPhone",
                    Sku = "SKU1",
                    Ean = "111",
                    BasePrice = 1000,
                    AvailableStock = 5,
                    BrandId = brand.Id,
                    CategoryId = category.Id
                });

            Func<Task> act = async () =>
                await service.CreateProductAsync(
                    new CreateProductDto
                    {
                        Name = "iPhone 2",
                        Sku = "SKU1",
                        Ean = "222",
                        BasePrice = 1000,
                        AvailableStock = 5,
                        BrandId = brand.Id,
                        CategoryId = category.Id
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateProduct_Should_Throw_When_Ean_Already_Exists()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();
            var category = await CreateCategoryAsync();

            await service.CreateProductAsync(
                new CreateProductDto
                {
                    Name = "iPhone",
                    Sku = "ABC",
                    Ean = "111",
                    BasePrice = 1000,
                    AvailableStock = 5,
                    BrandId = brand.Id,
                    CategoryId = category.Id
                });

            Func<Task> act = async () =>
                await service.CreateProductAsync(
                    new CreateProductDto
                    {
                        Name = "iPhone 2",
                        Sku = "DEF",
                        Ean = "111",
                        BasePrice = 1000,
                        AvailableStock = 5,
                        BrandId = brand.Id,
                        CategoryId = category.Id
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>();
        }
    }
}

