using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductServiceIntegrationTests
{
    public class ProductReadIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetAllProducts_Should_Only_Return_Active_For_Non_Admin()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(
                name: "Active",
                sku: "ACTIVE",
                ean: "1234567890123",
                isActive: true);

            await CreateProductAsync(
                name: "Inactive",
                sku: "INACTIVE",
                ean: "1234567890124",
                isActive: false);

            var result = await service.GetAllProductsAsync(null, isAdmin: false);

            result.Should().ContainSingle();
            result.Single().Name.Should().Be("Active");
        }

        [Fact]
        public async Task GetAllProducts_Should_Return_All_For_Admin()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(
                name: "iPhone 1",
                sku: "SKU001",
                ean: "1234567890123",
                isActive: true);

            await CreateProductAsync(
                name: "iPhone 2",
                sku: "SKU002",
                ean: "1234567890124",
                isActive: false);

            var result = await service.GetAllProductsAsync(null, isAdmin: true);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProductById_Should_Return_Product()
        {
            var service = GetService<ProductService>();

            var brand = await CreateBrandAsync();
            var category = await CreateCategoryAsync();

            var product = await CreateProductAsync(
                name: "iPhone",
                sku: "SKU1",
                ean: "1234567890123",
                basePrice: 1000,
                brandId: brand.Id,
                categoryId: category.Id);

            product.Description = "Icecat description";
            product.SpecificationsJson = "{\"color\":\"black\"}";
            product.ContentSource = "Icecat";
            product.IcecatProductId = "ICECAT-123";
            product.IcecatLastSynced = new DateTime(2026, 1, 1);

            await Context.SaveChangesAsync();

            var result = await service.GetProductByIdAsync(product.Id, null, isAdmin: true);

            result.Should().NotBeNull();
            result!.Name.Should().Be("iPhone");
            result.Description.Should().Be("Icecat description");
            result.SpecificationsJson.Should().Be("{\"color\":\"black\"}");
            result.ContentSource.Should().Be("Icecat");
            result.IcecatProductId.Should().Be("ICECAT-123");
            result.IcecatLastSynced.Should().Be(new DateTime(2026, 1, 1));
        }

        [Fact]
        public async Task GetProductById_Should_Throw_When_Not_Found()
        {
            var service = GetService<ProductService>();

            Func<Task> act = async () =>
                await service.GetProductByIdAsync(999, null, isAdmin: true);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task GetProductsAsync_Should_Filter_By_ContentSource()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(name: "Manual Product");
            await CreateProductAsync(name: "Rackbeat Product");

            var manualProduct = await Context.Products.SingleAsync(x => x.Name == "Manual Product");
            manualProduct.ContentSource = "Manual";

            var rackbeatProduct = await Context.Products.SingleAsync(x => x.Name == "Rackbeat Product");
            rackbeatProduct.ContentSource = "Rackbeat";

            await Context.SaveChangesAsync();

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    ContentSource = "Manual",
                    Page = 1,
                    PageSize = 100
                },
                companyId: null,
                isAdmin: true);

            result.Items.Should().ContainSingle();
            result.Items.Single().ContentSource.Should().Be("Manual");
        }

        [Fact]
        public async Task GetProductsAsync_Should_Filter_By_ContentLocked()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(name: "Locked Product");
            await CreateProductAsync(name: "Unlocked Product");

            var locked = await Context.Products.SingleAsync(x => x.Name == "Locked Product");
            locked.ContentLocked = true;

            var unlocked = await Context.Products.SingleAsync(x => x.Name == "Unlocked Product");
            unlocked.ContentLocked = false;

            await Context.SaveChangesAsync();

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    ContentLocked = true,
                    Page = 1,
                    PageSize = 100
                },
                companyId: null,
                isAdmin: true);

            result.Items.Should().ContainSingle();
            result.Items.Single().ContentLocked.Should().BeTrue();
        }

        [Fact]
        public async Task GetProductsAsync_Should_Filter_By_HasIcecatProductId()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(name: "Icecat Product");
            await CreateProductAsync(name: "No Icecat Product");

            var icecatProduct = await Context.Products.SingleAsync(x => x.Name == "Icecat Product");
            icecatProduct.IcecatProductId = "ICECAT-123";

            var noIcecatProduct = await Context.Products.SingleAsync(x => x.Name == "No Icecat Product");
            noIcecatProduct.IcecatProductId = null;

            await Context.SaveChangesAsync();

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    HasIcecatProductId = true,
                    Page = 1,
                    PageSize = 100
                },
                companyId: null,
                isAdmin: true);

            result.Items.Should().ContainSingle();
            result.Items.Single().IcecatProductId.Should().Be("ICECAT-123");
        }

        [Fact]
        public async Task GetProductsAsync_Should_Filter_By_HasContent()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(name: "Content Product");
            await CreateProductAsync(name: "No Content Product");

            var contentProduct = await Context.Products.SingleAsync(x => x.Name == "Content Product");
            contentProduct.Description = "Product description";

            var noContentProduct = await Context.Products.SingleAsync(x => x.Name == "No Content Product");
            noContentProduct.Description = null;
            noContentProduct.SpecificationsJson = null;

            await Context.SaveChangesAsync();

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    HasContent = true,
                    Page = 1,
                    PageSize = 100
                },
                companyId: null,
                isAdmin: true);

            result.Items.Should().ContainSingle();
            result.Items.Single().Description.Should().Be("Product description");
        }

        // Icecat integration tests
        [Fact]
        public async Task GetProductsAsync_Should_Filter_By_Missing_Content()
        {
            var service = GetService<ProductService>();

            var missingContentProduct =
                await CreateProductAsync(
                    name: "Missing Content Product"
                );

            var descriptionProduct =
                await CreateProductAsync(
                    name: "Description Product"
                );

            descriptionProduct.Description =
                "Product description";

            var specificationsProduct =
                await CreateProductAsync(
                    name: "Specifications Product"
                );

            specificationsProduct.SpecificationsJson =
                "[{\"groupName\":\"General\",\"items\":[]}]";

            await Context.SaveChangesAsync();

            var result =
                await service.GetProductsAsync(
                    new ProductQueryParameters
                    {
                        HasContent = false,
                        Page = 1,
                        PageSize = 100
                    },
                    companyId: null,
                    isAdmin: true
                );

            result.Items.Should().ContainSingle();

            result.Items
                .Single()
                .Id
                .Should()
                .Be(missingContentProduct.Id);
        }

        [Fact]
        public async Task GetProductsAsync_Should_Return_TotalCount_For_Missing_Content()
        {
            var service = GetService<ProductService>();

            await CreateProductAsync(
                name: "Missing Content Product 1"
            );

            await CreateProductAsync(
                name: "Missing Content Product 2"
            );

            var contentProduct =
                await CreateProductAsync(
                    name: "Content Product"
                );

            contentProduct.Description =
                "Product description";

            await Context.SaveChangesAsync();

            var result =
                await service.GetProductsAsync(
                    new ProductQueryParameters
                    {
                        HasContent = false,
                        Page = 1,
                        PageSize = 1
                    },
                    companyId: null,
                    isAdmin: true
                );

            result.Items.Should().HaveCount(1);
            result.TotalCount.Should().Be(2);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(1);
        }

        [Fact]
        public async Task GetProductById_Should_Use_IcecatName_When_Available()
        {
            var service = GetService<ProductService>();

            var product = await CreateProductAsync(
                name: "Rackbeat product name",
                sku: "ICECAT-NAME-001",
                ean: "1234567890201");

            product.IcecatName = "Improved Icecat product name";

            await Context.SaveChangesAsync();

            var result = await service.GetProductByIdAsync(
                product.Id,
                companyId: null,
                isAdmin: true);

            result.Should().NotBeNull();

            result!.Name.Should().Be("Improved Icecat product name");

            result.IcecatName.Should().Be("Improved Icecat product name");

            ResetContext();

            var storedProduct = await Context.Products.SingleAsync(x => x.Id == product.Id);

            storedProduct.Name.Should().Be("Rackbeat product name");

            storedProduct.IcecatName.Should().Be("Improved Icecat product name");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetProductById_Should_Fall_Back_To_ProductName_When_IcecatName_Is_Blank(
            string? icecatName)
        {
            var service = GetService<ProductService>();

            var product = await CreateProductAsync(
                name: "Rackbeat fallback name",
                sku: $"FALLBACK-{Guid.NewGuid()}");

            product.IcecatName = icecatName;

            await Context.SaveChangesAsync();

            var result = await service.GetProductByIdAsync(
                product.Id,
                companyId: null,
                isAdmin: true);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Rackbeat fallback name");
            result.IcecatName.Should().Be(icecatName);
        }

        [Fact]
        public async Task GetProductsAsync_Should_Search_By_IcecatName()
        {
            var service = GetService<ProductService>();

            var matchingProduct = await CreateProductAsync(
                name: "Original product name",
                sku: "SEARCH-ICECAT-001",
                ean: "1234567890203");

            matchingProduct.IcecatName = "Zebra passive barcode scanner holder";

            var otherProduct = await CreateProductAsync(
                name: "Completely different product",
                sku: "SEARCH-OTHER-001",
                ean: "1234567890204");

            otherProduct.IcecatName = "Ordinary office monitor";

            await Context.SaveChangesAsync();

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    Search = "barcode scanner holder",
                    Page = 1,
                    PageSize = 100
                },
                companyId: null,
                isAdmin: true);

            result.Items.Should().ContainSingle();

            var returnedProduct = result.Items.Single();

            returnedProduct.Id.Should().Be(matchingProduct.Id);

            returnedProduct.Name.Should().Be("Zebra passive barcode scanner holder");

            returnedProduct.IcecatName.Should().Be("Zebra passive barcode scanner holder");
        }

        [Fact]
        public async Task GetProductsAsync_Should_Sort_By_IcecatName_With_ProductName_Fallback()
        {
            var service = GetService<ProductService>();

            var firstByIcecatName =
                await CreateProductAsync(
                    name: "Zulu Rackbeat name",
                    sku: "SORT-001",
                    ean: "1234567890205");

            firstByIcecatName.IcecatName = "Alpha Icecat name";

            var secondByFallbackName =
                await CreateProductAsync(
                    name: "Beta Rackbeat name",
                    sku: "SORT-002",
                    ean: "1234567890206");

            secondByFallbackName.IcecatName = null;

            var thirdByIcecatName =
                await CreateProductAsync(
                    name: "Alpha Rackbeat name",
                    sku: "SORT-003",
                    ean: "1234567890207");

            thirdByIcecatName.IcecatName = "Charlie Icecat name";

            await Context.SaveChangesAsync();

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    SortBy = "name",
                    SortDirection = "asc",
                    Page = 1,
                    PageSize = 100
                },
                companyId: null,
                isAdmin: true);

            result.Items
                .Select(x => x.Name)
                .Should()
                .ContainInOrder(
                    "Alpha Icecat name",
                    "Beta Rackbeat name",
                    "Charlie Icecat name");
        }

        [Fact]
        public async Task GetProductsAsync_Should_Sort_By_DisplayName_Descending()
        {
            var service = GetService<ProductService>();

            var first = await CreateProductAsync(
                name: "Original A",
                sku: "SORT-DESC-001",
                ean: "1234567890208");

            first.IcecatName = "Alpha name";

            var second = await CreateProductAsync(
                name: "Original B",
                sku: "SORT-DESC-002",
                ean: "1234567890209");

            second.IcecatName = "Zulu name";

            await Context.SaveChangesAsync();

            var result = await service.GetProductsAsync(
                new ProductQueryParameters
                {
                    SortBy = "name",
                    SortDirection = "desc",
                    Page = 1,
                    PageSize = 100
                },
                companyId: null,
                isAdmin: true);

            result.Items
                .Select(x => x.Name)
                .Should()
                .Equal(
                    "Zulu name",
                    "Alpha name");
        }
    }
}

