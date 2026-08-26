using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductImportServiceIntegrationTests
{
    public class ImportProductsFromCSVIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ImportCsvAsync_Should_Create_Product()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Test Product;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

            var result = await service.ImportCsvAsync(stream);

            result.Created.Should().Be(1);
            result.Updated.Should().Be(0);
            result.Skipped.Should().Be(0);

            var product = await Context.Products.Include(x => x.Brand).Include(x => x.Category).SingleAsync();

            product.Sku.Should().Be("SKU001");
            product.Name.Should().Be("Test Product");
            product.AvailableStock.Should().Be(10);
            product.PurchasedQuantity.Should().Be(0);
            product.Ean.Should().Be("1234567890123");
            product.BasePrice.Should().Be(100);
            product.IsActive.Should().BeTrue();
            product.Brand.Should().NotBeNull();
            product.Brand.Should().NotBeNull();
            product.Category.Should().NotBeNull();
            product.Category.Should().NotBeNull();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Update_Existing_Product()
        {
            var service = GetService<ProductImportService>();

            await CreateProductAsync(
                name: "Old Product",
                sku: "SKU001",
                ean: "1234567890123");

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Updated Product;10;0;1234567890123;200;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            var result = await service.ImportCsvAsync(stream);

            result.Updated.Should().Be(1);

            var updated = await Context.Products.SingleAsync(x => x.Sku == "SKU001");

            updated.Name.Should().Be("Updated Product");
            updated.BasePrice.Should().Be(200);
            updated.Ean.Should().Be("1234567890123");
            updated.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Skip_Product_With_Empty_Sku()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                ;Test Product;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await FluentActions
                .Invoking(() => service.ImportCsvAsync(stream))
                .Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Create_Brand_And_Category()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Test Product;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            Context.Brands.Should().ContainSingle();

            Context.Categories.Should().ContainSingle();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Skip_Product_With_Zero_Stock_And_Zero_Price()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Test Product;0;0;1234567890123;0;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await FluentActions
                .Invoking(() => service.ImportCsvAsync(stream))
                .Should()
                .ThrowAsync<InvalidOperationException>();

            Context.Products.Should().BeEmpty();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Normalize_Negative_Stock_To_Zero()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Test Product;-5;10;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var product = await Context.Products.SingleAsync();

            product.AvailableStock.Should().Be(0);
            product.PurchasedQuantity.Should().Be(10);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Clear_DeliveryDate_When_PurchasedQuantity_Is_Zero()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Test Product;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var product = await Context.Products.SingleAsync();

            product.ExpectedDeliveryDate.Should().BeNull();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Create_New_Brand_Only_Once()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product1;10;0;1234567890123;100;Logitech;Mus
                SKU002;Product2;10;0;1234567890124;100;Logitech;Tastatur
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            Context.Brands.Count()
                .Should().Be(1);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Create_New_Category_Only_Once()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product1;10;0;1234567890123;100;Logitech;Mus
                SKU002;Product2;10;0;1234567890124;100;Razer;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            Context.Categories.Count()
                .Should().Be(1);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Reuse_Existing_Brand()
        {
            Context.Brands.Add(new Brand
            {
                Name = "Logitech"
            });

            await Context.SaveChangesAsync();

            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product1;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            Context.Brands.Count()
                .Should().Be(1);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Skip_DamagedBox_Product()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001_DB;Test Product;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(Encoding.UTF8.GetBytes(csv));

            await FluentActions
                .Invoking(() => service.ImportCsvAsync(stream))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Import aborted because no valid products were imported.");
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Normalize_Scientific_Ean()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product;10;0;1.23457E+12;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var product = await Context.Products.SingleAsync();

            product.Ean.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Skip_Duplicate_Ean()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product1;10;0;1234567890123;100;Logitech;Mus
                SKU002;Product2;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            var result = await service.ImportCsvAsync(stream);

            result.Created.Should().Be(1);
            result.Skipped.Should().Be(1);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Deactivate_Missing_Products()
        {
            await CreateProductAsync(
                sku: "OLDSKU",
                ean: "9999999999999");

            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                NEWSKU;Product;10;0;1234567890123;100;Logitech;Mus
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var oldProduct = await Context.Products.SingleAsync(x => x.Sku == "OLDSKU");

            oldProduct.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Create_Product_Image()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product;10;0;1234567890123;100;Logitech;Mus;https://test.dk/image.jpg
                """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            ResetContext();

            var images = await Context.ProductImages.ToListAsync();

            images.Should().ContainSingle();

            var image = images.Single();

            image.Url.Should().Be("https://test.dk/image.jpg");
            image.Source.Should().Be("Rackbeat");
            image.ExternalId.Should().Be("https://test.dk/image.jpg");
            image.LastSynced.Should().NotBeNull();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Set_First_Image_As_Primary()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product;10;0;1234567890123;100;Logitech;Mus;https://a.jpg|https://b.jpg
                """;

            using var stream =
                new MemoryStream(Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            ResetContext();

            var images = await Context.ProductImages.OrderBy(x => x.Id).ToListAsync();

            images.Count.Should().Be(2);

            images.First(x => x.Url.Contains("a.jpg")).IsPrimary.Should().BeTrue();
            images.First(x => x.Url.Contains("b.jpg")).IsPrimary.Should().BeFalse();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Skip_Product_With_Missing_Required_Fields()
        {
            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Product;10;0;;100;;
                """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

            await FluentActions
                .Invoking(() => service.ImportCsvAsync(stream))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Import aborted because no valid products were imported.");

            Context.Products.Should().BeEmpty();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Not_Add_Rackbeat_Image_When_Product_Already_Has_Image()
        {
            var product = await CreateProductAsync(
                sku: "SKU001",
                ean: "1234567890123");

            Context.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                Url = "https://manual.dk/image.jpg",
                Source = "Manual",
                IsPrimary = true
            });

            await Context.SaveChangesAsync();

            var service = GetService<ProductImportService>();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Updated Product;10;0;1234567890123;100;Logitech;Mus;https://rackbeat.dk/image.jpg
                """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            ResetContext();

            var images = await Context.ProductImages.ToListAsync();

            images.Should().ContainSingle();
            images.Single().Url.Should().Be("https://manual.dk/image.jpg");
            images.Single().Source.Should().Be("Manual");
        }

        // Icecat included in the import
        [Fact]
        public async Task ImportCsvAsync_Should_Not_Overwrite_EnrichedContentFields()
        {
            var service = GetService<ProductImportService>();

            var product = new Product
            {
                Sku = "SKU001",
                Name = "Old Product",
                IcecatName = "Existing Icecat product name",
                Description = "Manual description",
                SpecificationsJson = "{\"color\":\"black\"}",
                ContentSource = "Manual",
                ContentLocked = true,
                IcecatProductId = "ICECAT-123",
                IcecatLastSynced = new DateTime(2026, 1, 1)
            };

            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var csv = """
                Sku;Name;Available;Purchased;Ean;Price (EUR);Brand;Category;ImageUrl
                SKU001;Updated Product;10;0;1234567890123;200;Logitech;Mus
                """;

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Sku == "SKU001");

            updated.Name.Should().Be("Updated Product");
            updated.IcecatName.Should().Be("Existing Icecat product name");
            updated.BasePrice.Should().Be(200);
            updated.Description.Should().Be("Manual description");
            updated.SpecificationsJson.Should().Be("{\"color\":\"black\"}");
            updated.ContentSource.Should().Be("Manual");
            updated.ContentLocked.Should().BeTrue();
            updated.IcecatProductId.Should().Be("ICECAT-123");
            updated.IcecatLastSynced.Should().Be(new DateTime(2026, 1, 1));
        }
    }
}

