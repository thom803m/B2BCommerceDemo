using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Tests.Integration.Shared;
using B2BCommerceDemo.Tests.Integration.Shared.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Integration.Integrations.Rackbeat.ProductSync
{
    public class RackbeatProductSyncIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task SyncProductsAsync_Should_Create_Products_Returned_By_Rackbeat()
        {
            var service = GetService<IRackbeatProductSyncService>();

            var rackbeatProducts = RackbeatTestDataFactory.CreateProductList();

            RackbeatClientMock
                .Setup(x => x.GetProductsForImportAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(rackbeatProducts);

            var result = await service.SyncProductsAsync();

            result.Created.Should().Be(2);
            result.Updated.Should().Be(0);
            result.Skipped.Should().Be(0);

            ResetContext();

            var products = 
                await Context.Products.Include(x => x.Brand).Include(x => x.Category).OrderBy(x => x.Sku).ToListAsync();

            products.Should().HaveCount(2);

            products
                .Select(x => x.Sku)
                .Should()
                .Equal("SKU-001", "SKU-002");

            products.Should().OnlyContain(
                x => x.ContentSource == "Rackbeat");

            products.Should().OnlyContain(
                x => x.ContentLocked == false);

            products.Should().OnlyContain(
                x => x.Brand != null &&
                     x.Brand.Name == "Zebra");

            products.Should().OnlyContain(
                x => x.Category != null &&
                     x.Category.Name == "Barcode Scanner");

            RackbeatClientMock.Verify(
                x => x.GetProductsForImportAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SyncProductsAsync_Should_Update_Commercial_Fields_Without_Overwriting_Icecat_Content()
        {
            var service = GetService<IRackbeatProductSyncService>();

            var existingProduct =
                await CreateProductAsync(
                    name: "Old Rackbeat name",
                    sku: "SKU-001",
                    ean: "1234567890123",
                    basePrice: 100m,
                    stock: 5);

            existingProduct.IcecatName = "Existing Icecat product name";

            existingProduct.Description = "Existing Icecat description";

            existingProduct.SpecificationsJson = "{\"colour\":\"black\"}";

            existingProduct.IcecatProductId = "ICECAT-123";

            existingProduct.IcecatLastSynced = new DateTime(2026, 1, 1);

            existingProduct.ContentSource = "Icecat";
            existingProduct.ContentLocked = false;

            await Context.SaveChangesAsync();

            var rackbeatProduct =
                RackbeatTestDataFactory.CreateProduct(
                    sku: "SKU-001",
                    name: "Updated Rackbeat name",
                    ean: "1234567890123",
                    basePrice: 250m,
                    availableStock: 25,
                    purchasedQuantity: 4,
                    expectedDeliveryDate: new DateTime(2026, 8, 15));

            RackbeatClientMock
                .Setup(x => x.GetProductsForImportAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([rackbeatProduct]);

            var result = await service.SyncProductsAsync();

            result.Created.Should().Be(0);
            result.Updated.Should().Be(1);
            result.Skipped.Should().Be(0);

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == existingProduct.Id);

            updated.Name.Should().Be("Updated Rackbeat name");

            updated.BasePrice.Should().Be(250m);
            updated.AvailableStock.Should().Be(25);
            updated.PurchasedQuantity.Should().Be(4);

            updated.ExpectedDeliveryDate.Should().Be(new DateTime(2026, 8, 15));

            updated.IcecatName.Should().Be("Existing Icecat product name");

            updated.Description.Should().Be("Existing Icecat description");

            updated.SpecificationsJson.Should().Be("{\"colour\":\"black\"}");

            updated.IcecatProductId.Should().Be("ICECAT-123");

            updated.IcecatLastSynced.Should().Be(new DateTime(2026, 1, 1));

            updated.ContentSource.Should().Be("Icecat");

            updated.ContentLocked.Should().BeFalse();
        }

        [Fact]
        public async Task SyncProductsAsync_Should_Forward_CancellationToken_To_RackbeatClient()
        {
            var service = GetService<IRackbeatProductSyncService>();

            using var cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            RackbeatClientMock
                .Setup(x => x.GetProductsForImportAsync(cancellationToken))
                .ReturnsAsync(RackbeatTestDataFactory.CreateProductList());

            await service.SyncProductsAsync(cancellationToken);

            RackbeatClientMock.Verify(
                x => x.GetProductsForImportAsync(cancellationToken),
                Times.Once);
        }
    }
}
