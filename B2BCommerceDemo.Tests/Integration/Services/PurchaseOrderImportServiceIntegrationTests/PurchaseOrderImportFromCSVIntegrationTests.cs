using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Tests.Integration.Services.PurchaseOrderImportServiceIntegrationTests
{
    public class PurchaseOrderImportFromCSVIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ImportCsvAsync_Should_Update_ExpectedDeliveryDate()
        {
            var service = GetService<PurchaseOrderImportService>();

            var product = await CreateProductAsync(
                sku: "ABC123",
                ean: "1234567890123");

            product.PurchasedQuantity = 10;

            await Context.SaveChangesAsync();

            var csv =
                """
                Sku;PreferredDeliveryDate;Quantity;Invoiced quantity
                ABC123;15-07-2026;10,00;0,00
                """;

            using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var updated = await Context.Products.SingleAsync(p => p.Id == product.Id);

            updated.ExpectedDeliveryDate.Should()
                .Be(new DateTime(2026, 7, 15));
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Update_LastSynced()
        {
            var service = GetService<PurchaseOrderImportService>();

            var product = await CreateProductAsync(
                sku: "ABC123",
                ean: "1234567890123");

            product.PurchasedQuantity = 10;

            await Context.SaveChangesAsync();

            var csv =
                """
                Sku;PreferredDeliveryDate
                ABC123;15-07-2026
                """;

            using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var updated = await Context.Products.SingleAsync(p => p.Id == product.Id);

            updated.LastSynced.Should().BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Ignore_Unknown_Sku()
        {
            var service = GetService<PurchaseOrderImportService>();

            var csv =
                """
                Sku;PreferredDeliveryDate
                UNKNOWN;15-07-2026
                """;

            using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            Context.Products.Should().BeEmpty();
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Use_Earliest_DeliveryDate_When_Duplicate_Sku()
        {
            var service = GetService<PurchaseOrderImportService>();

            var product = await CreateProductAsync(
                sku: "ABC123",
                ean: "1234567890123");

            product.PurchasedQuantity = 10;

            await Context.SaveChangesAsync();

            var csv =
                """
                Sku;PreferredDeliveryDate;Quantity;Invoiced quantity
                ABC123;20-07-2026;10,00;0,00
                ABC123;10-07-2026;10,00;0,00
                ABC123;15-07-2026;10,00;0,00
                """;

            using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var updated = await Context.Products.SingleAsync(p => p.Id == product.Id);

            updated.ExpectedDeliveryDate.Should()
                .Be(new DateTime(2026, 7, 10));
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Ignore_Rows_Without_DeliveryDate()
        {
            var service = GetService<PurchaseOrderImportService>();

            var product = await CreateProductAsync(
                sku: "ABC123",
                ean: "1234567890123");

            product.ExpectedDeliveryDate = null;

            await Context.SaveChangesAsync();

            var csv =
                """
                Sku;PreferredDeliveryDate
                ABC123;
                """;

            using var stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(csv));

            await service.ImportCsvAsync(stream);

            var updated = await Context.Products.SingleAsync(p => p.Id == product.Id);

            updated.ExpectedDeliveryDate.Should().BeNull();
        }
    }
}

