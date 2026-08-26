using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;
using System.Text;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductExportServiceIntegrationTests
{
    public class ExportProductsToCSVIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Return_Csv_Data()
        {
            var service = GetService<ProductExportService>();

            await CreateProductAsync(name: "Product A");
            await CreateProductAsync(name: "Product B");

            var result = await service.ExportProductsToCsvAsync();

            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Throw_When_No_Products()
        {
            var service = GetService<ProductExportService>();

            var act = () => service.ExportProductsToCsvAsync();

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("No active products found.");
        }

        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Only_Include_Active_Products()
        {
            var service = GetService<ProductExportService>();

            await CreateProductAsync(name: "Active", isActive: true);
            await CreateProductAsync(name: "Inactive", isActive: false);

            var result = await service.ExportProductsToCsvAsync();

            var csv = System.Text.Encoding.UTF8.GetString(result);

            csv.Should().Contain("Active");
            csv.Should().NotContain("Inactive");
        }

        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Filter_Stock_When_Stock_Field_Selected()
        {
            var service = GetService<ProductExportService>();

            await CreateProductAsync(name: "InStock", stock: 10);
            await CreateProductAsync(name: "NoStock", stock: 0);

            var result = await service.ExportProductsToCsvAsync(
                new List<string> { "name", "stock" });

            var csv = Encoding.UTF8.GetString(result);

            csv.Should().Contain("InStock");
            csv.Should().NotContain("NoStock");
        }

        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Filter_Purchase_When_Purchase_Field_Selected()
        {
            var service = GetService<ProductExportService>();

            var incoming = await CreateProductAsync(name: "Incoming");
            incoming.PurchasedQuantity = 5;

            var noIncoming = await CreateProductAsync(name: "NoIncoming");
            noIncoming.PurchasedQuantity = 0;

            await Context.SaveChangesAsync();

            var result = await service.ExportProductsToCsvAsync(
                new List<string> { "name", "purchase" });

            var csv = Encoding.UTF8.GetString(result);

            csv.Should().Contain("Incoming");
            csv.Should().NotContain("NoIncoming");
        }

        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Throw_When_Invalid_Field()
        {
            var service = GetService<ProductExportService>();

            await CreateProductAsync();

            var act = () => service.ExportProductsToCsvAsync(
                selectedFields: new List<string> { "invalid_field" });

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Invalid export fields: invalid_field");
        }

        // IcecatName is used for ordering
        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Use_IcecatName_When_Available()
        {
            var service = GetService<ProductExportService>();

            var product = await CreateProductAsync(
                name: "Rackbeat product name",
                sku: "EXPORT-ICECAT-001",
                ean: "1234567890301");

            product.IcecatName = "Improved Icecat product name";

            await Context.SaveChangesAsync();

            var result =
                await service.ExportProductsToCsvAsync(
                    new List<string>
                    {
                        "sku",
                        "name"
                    });

            var csv = Encoding.UTF8.GetString(result);

            csv.Should().Contain("Improved Icecat product name");

            csv.Should().NotContain( "Rackbeat product name");
        }

        [Fact]
        public async Task ExportProductsToCsvAsync_Should_Fall_Back_To_ProductName_When_IcecatName_Is_Missing()
        {
            var service = GetService<ProductExportService>();

            var product = await CreateProductAsync(
                name: "Rackbeat fallback name",
                sku: "EXPORT-FALLBACK-001",
                ean: "1234567890302");

            product.IcecatName = null;

            await Context.SaveChangesAsync();

            var result =
                await service.ExportProductsToCsvAsync(
                    new List<string>
                    {
                        "sku",
                        "name"
                    });

            var csv = Encoding.UTF8.GetString(result);

            csv.Should().Contain("Rackbeat fallback name");
        }
    }
}

