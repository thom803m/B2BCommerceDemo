using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Tests.Integration.Shared;
using B2BCommerceDemo.Tests.Integration.Shared.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Integration.Integrations.Rackbeat.PurchaseOrderSync
{
    public class RackbeatPurchaseOrderSyncIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task SyncExpectedDeliveriesAsync_Should_Update_Product_With_Earliest_DeliveryDate()
        {
            var service = GetService<IRackbeatPurchaseOrderSyncService>();

            var product = await CreateProductAsync(
                sku: "SKU-001",
                ean: "1234567890401");

            product.PurchasedQuantity = 10;

            await Context.SaveChangesAsync();

            var earliestDeliveryDate = DateTime.UtcNow.Date.AddDays(10);

            var laterDeliveryDate = DateTime.UtcNow.Date.AddDays(20);

            var deliveries =
                new[]
                {
                    RackbeatTestDataFactory
                        .CreateExpectedDelivery(
                            sku: "SKU-001",
                            expectedDeliveryDate: laterDeliveryDate),

                    RackbeatTestDataFactory
                        .CreateExpectedDelivery(
                            sku: "SKU-001",
                            expectedDeliveryDate: earliestDeliveryDate)
                }
                .ToList();

            RackbeatClientMock
                .Setup(x => x.GetExpectedDeliveriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(deliveries);

            var result = await service.SyncExpectedDeliveriesAsync();

            result.Updated.Should().Be(1);
            result.Warnings.Should().BeEmpty();

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.ExpectedDeliveryDate.Should().Be(earliestDeliveryDate);

            RackbeatClientMock.Verify(
                x => x.GetExpectedDeliveriesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SyncExpectedDeliveriesAsync_Should_Not_Set_DeliveryDate_When_Product_Has_No_Purchased_Quantity()
        {
            var service = GetService<IRackbeatPurchaseOrderSyncService>();

            var product = await CreateProductAsync(
                sku: "SKU-001",
                ean: "1234567890402");

            product.PurchasedQuantity = 0;
            product.ExpectedDeliveryDate = DateTime.UtcNow.Date.AddDays(5);

            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.GetExpectedDeliveriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    RackbeatTestDataFactory
                        .CreateExpectedDelivery(
                            sku: "SKU-001",
                            expectedDeliveryDate:
                                DateTime.UtcNow.Date
                                    .AddDays(15))
                ]);

            var result = await service.SyncExpectedDeliveriesAsync();

            result.Updated.Should().Be(1);

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.ExpectedDeliveryDate.Should().BeNull();
        }

        [Fact]
        public async Task SyncExpectedDeliveriesAsync_Should_Clear_Date_And_Add_Warning_When_Product_Is_Missing_From_Rackbeat_Deliveries()
        {
            var service = GetService<IRackbeatPurchaseOrderSyncService>();

            var product = await CreateProductAsync(
                sku: "SKU-MISSING",
                ean: "1234567890403");

            product.PurchasedQuantity = 5;
            product.ExpectedDeliveryDate = DateTime.UtcNow.Date.AddDays(10);

            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.GetExpectedDeliveriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    RackbeatTestDataFactory
                        .CreateExpectedDelivery(
                            sku: "OTHER-SKU",
                            expectedDeliveryDate:
                                DateTime.UtcNow.Date
                                    .AddDays(20))
                ]);

            var result = await service.SyncExpectedDeliveriesAsync();

            result.Updated.Should().Be(1);

            result.Warnings.Should().ContainSingle(
                warning =>
                    warning.Contains("SKU-MISSING") &&
                    warning.Contains("no expected delivery date"));

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.ExpectedDeliveryDate.Should().BeNull();
        }

        [Fact]
        public async Task SyncExpectedDeliveriesAsync_Should_Preserve_Existing_Dates_When_Rackbeat_Returns_No_Deliveries()
        {
            var service = GetService<IRackbeatPurchaseOrderSyncService>();

            var existingDeliveryDate = DateTime.UtcNow.Date.AddDays(10);

            var product = await CreateProductAsync(
                sku: "SKU-001",
                ean: "1234567890404");

            product.PurchasedQuantity = 5;
            product.ExpectedDeliveryDate = existingDeliveryDate;

            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.GetExpectedDeliveriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await service.SyncExpectedDeliveriesAsync();

            result.Updated.Should().Be(0);

            result.Warnings.Should().ContainSingle(
                "No expected deliveries were returned from Rackbeat. Existing purchased quantities were not changed.");

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.ExpectedDeliveryDate.Should().Be(existingDeliveryDate);
        }

        [Fact]
        public async Task SyncExpectedDeliveriesAsync_Should_Forward_CancellationToken_To_RackbeatClient()
        {
            var service = GetService<IRackbeatPurchaseOrderSyncService>();

            using var cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            RackbeatClientMock
                .Setup(x => x.GetExpectedDeliveriesAsync(cancellationToken))
                .ReturnsAsync([]);

            await service.SyncExpectedDeliveriesAsync(cancellationToken);

            RackbeatClientMock.Verify(
                x => x.GetExpectedDeliveriesAsync(cancellationToken),
                Times.Once);
        }
    }
}
