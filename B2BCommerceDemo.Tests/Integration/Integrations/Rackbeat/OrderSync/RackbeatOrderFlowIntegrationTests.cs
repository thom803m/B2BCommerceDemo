using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Events.Handlers.Orders.Rackbeat;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace B2BCommerceDemo.Tests.Integration.Integrations.Rackbeat.OrderSync
{
    public class RackbeatOrderFlowIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateFromCartAsync_Should_Create_Order_And_Sync_To_Rackbeat_When_Handler_Runs()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active,
                RackbeatCustomerNumber = "900000580"
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(
                name: "Test Product",
                sku: "SKU-001",
                basePrice: 100m,
                stock: 10);

            Context.Carts.Add(new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items =
                [
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 2
                    }
                ]
            });

            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.CreateOrderAsync(
                    It.IsAny<Order>(),
                    "900000580",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("RB-1001");

            var orderService = GetService<OrderService>();

            await orderService.CreateFromCartAsync(
                company.Id,
                user.Id,
                "idem-rackbeat-flow-1");

            var createdEvent = EventDispatcher
                .GetEvents<OrderCreatedEvent>()
                .Single();

            var handler = new OrderCreatedRackbeatHandler(
                GetService<IRackbeatOrderSyncService>(),
                NullLogger<OrderCreatedRackbeatHandler>.Instance);

            await handler.HandleAsync(createdEvent);

            ResetContext();

            var order = await Context.Orders.Include(o => o.Items).SingleAsync();

            order.RackbeatOrderNumber.Should().Be("RB-1001");
            order.RackbeatSyncStatus.Should().Be(RackbeatSyncStatus.Synced);
            order.RackbeatSyncError.Should().BeNull();
            order.RackbeatSyncedAt.Should().NotBeNull();

            RackbeatClientMock.Verify(x => x.CreateOrderAsync(
                It.Is<Order>(o =>
                    o.Id == order.Id &&
                    o.Items.Count == 1 &&
                    o.Items.Single().Sku == "SKU-001" &&
                    o.Items.Single().Quantity == 2),
                "900000580",
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SyncOrderAsync_Should_Create_Rackbeat_Order_And_Update_Order()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active,
                RackbeatCustomerNumber = "900000580"
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                Items =
                [
                    new OrderItem
                    {
                        ProductId = 1,
                        Sku = "SKU-001",
                        ProductName = "Test Product",
                        Quantity = 2,
                        UnitPrice = 100m
                    }
                ]
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.CreateOrderAsync(
                    It.IsAny<Order>(),
                    "900000580",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("RB-1001");

            var service = GetService<IRackbeatOrderSyncService>();

            var result = await service.SyncOrderAsync(order.Id);

            result.Created.Should().Be(1);
            result.Skipped.Should().Be(0);

            ResetContext();

            var updated = await Context.Orders.SingleAsync();

            updated.RackbeatOrderNumber.Should().Be("RB-1001");
            updated.RackbeatSyncStatus.Should().Be(RackbeatSyncStatus.Synced);
            updated.RackbeatSyncError.Should().BeNull();
            updated.RackbeatSyncedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task SyncOrderAsync_Should_Skip_When_Order_Is_Already_Synced()
        {
            var order = new Order
            {
                CompanyId = 1,
                UserId = "user-1",
                RackbeatOrderNumber = "RB-1001"
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var service = GetService<IRackbeatOrderSyncService>();

            var result = await service.SyncOrderAsync(order.Id);

            result.Created.Should().Be(0);
            result.Skipped.Should().Be(1);
            result.Warnings.Should().ContainSingle();

            RackbeatClientMock.Verify(x => x.CreateOrderAsync(
                It.IsAny<Order>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task SyncOrderAsync_Should_Mark_Failed_When_Company_Has_No_Rackbeat_Customer_Number()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active,
                RackbeatCustomerNumber = null
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1"
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var service = GetService<IRackbeatOrderSyncService>();

            var result = await service.SyncOrderAsync(order.Id);

            result.Created.Should().Be(0);
            result.Skipped.Should().Be(1);
            result.Warnings.Should().Contain(w =>
                w.Contains("Rackbeat sync failed"));

            ResetContext();

            var updated = await Context.Orders.SingleAsync();

            updated.RackbeatSyncStatus.Should().Be(RackbeatSyncStatus.Failed);
            updated.RackbeatSyncError.Should().Contain("has no Rackbeat customer number");

            RackbeatClientMock.Verify(x => x.CreateOrderAsync(
                It.IsAny<Order>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
