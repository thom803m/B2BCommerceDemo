using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Integration.Shared;
using B2BCommerceDemo.Tests.Integration.Shared.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Integration.Integrations.Rackbeat.OrderStatusSync
{
    public class RackbeatOrderStatusSyncIntegrationTests : IntegrationTestBase
    {
        [Theory]
        [InlineData(
            false,
            false,
            false,
            false,
            false,
            OrderStatus.Pending)]
        [InlineData(
            true,
            false,
            false,
            false,
            false,
            OrderStatus.Confirmed)]
        [InlineData(
            true,
            true,
            false,
            false,
            false,
            OrderStatus.Processing)]
        [InlineData(
            true,
            true,
            true,
            false,
            false,
            OrderStatus.Shipped)]
        [InlineData(
            true,
            true,
            true,
            true,
            false,
            OrderStatus.Completed)]
        [InlineData(
            true,
            true,
            true,
            true,
            true,
            OrderStatus.Cancelled)]
        public async Task SyncOrderStatusesAsync_Should_Map_Rackbeat_Status(
            bool isBooked,
            bool isReadyForShipping,
            bool isShipped,
            bool isInvoiced,
            bool isCancelled,
            OrderStatus expectedStatus)
        {
            var service = GetService<IRackbeatOrderStatusSyncService>();

            var company = await CreateCompanyAsync();

            var initialStatus =
                expectedStatus == OrderStatus.Pending
                    ? OrderStatus.Confirmed
                    : OrderStatus.Pending;

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                Status = initialStatus,
                RackbeatOrderNumber = "RB-1001"
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.GetOrderAsync("RB-1001", It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    RackbeatTestDataFactory
                        .CreateOrderResponse(
                            number: 1001,
                            isBooked: isBooked,
                            isCancelled: isCancelled,
                            isShipped: isShipped,
                            isInvoiced: isInvoiced,
                            isReadyForShipping: isReadyForShipping));

            var result = await service.SyncOrderStatusesAsync();

            result.Updated.Should().Be(1);
            result.Skipped.Should().Be(0);
            result.Warnings.Should().BeEmpty();

            ResetContext();

            var updated = await Context.Orders.SingleAsync(x => x.Id == order.Id);

            updated.Status.Should().Be(expectedStatus);
        }

        [Fact]
        public async Task SyncOrderStatusesAsync_Should_Skip_When_Status_Has_Not_Changed()
        {
            var service = GetService<IRackbeatOrderStatusSyncService>();

            var company = await CreateCompanyAsync();

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                Status = OrderStatus.Confirmed,
                RackbeatOrderNumber = "RB-1001"
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.GetOrderAsync("RB-1001", It.IsAny<CancellationToken>()))
                .ReturnsAsync(RackbeatTestDataFactory.CreateOrderResponse(number: 1001, isBooked: true));

            var result = await service.SyncOrderStatusesAsync();

            result.Updated.Should().Be(0);
            result.Skipped.Should().Be(1);
            result.Warnings.Should().BeEmpty();

            ResetContext();

            var unchanged = await Context.Orders.SingleAsync(x => x.Id == order.Id);

            unchanged.Status.Should().Be(OrderStatus.Confirmed);
        }

        [Fact]
        public async Task SyncOrderStatusesAsync_Should_Skip_When_Rackbeat_Order_Is_Not_Found()
        {
            var service = GetService<IRackbeatOrderStatusSyncService>();

            var company = await CreateCompanyAsync();

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                Status = OrderStatus.Pending,
                RackbeatOrderNumber = "RB-MISSING"
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.GetOrderAsync("RB-MISSING", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Core.DTOs.Integrations.Rackbeat.RackbeatOrderResponse?) null);

            var result = await service.SyncOrderStatusesAsync();

            result.Updated.Should().Be(0);
            result.Skipped.Should().Be(1);

            result.Warnings.Should().ContainSingle(
                warning =>
                    warning.Contains(order.Id.ToString()) &&
                    warning.Contains("RB-MISSING") &&
                    warning.Contains("was not found"));

            ResetContext();

            var unchanged = await Context.Orders.SingleAsync(x => x.Id == order.Id);

            unchanged.Status.Should().Be(OrderStatus.Pending);
        }

        [Fact]
        public async Task SyncOrderStatusesAsync_Should_Continue_When_One_Rackbeat_Request_Fails()
        {
            var service = GetService<IRackbeatOrderStatusSyncService>();

            var company = await CreateCompanyAsync();

            var failingOrder = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                Status = OrderStatus.Pending,
                RackbeatOrderNumber = "RB-FAIL"
            };

            var successfulOrder = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                Status = OrderStatus.Pending,
                RackbeatOrderNumber = "RB-SUCCESS"
            };

            Context.Orders.AddRange(failingOrder, successfulOrder);

            await Context.SaveChangesAsync();

            RackbeatClientMock
                .Setup(x => x.GetOrderAsync("RB-FAIL", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Rackbeat unavailable"));

            RackbeatClientMock
                .Setup(x => x.GetOrderAsync("RB-SUCCESS", It.IsAny<CancellationToken>()))
                .ReturnsAsync(RackbeatTestDataFactory.CreateOrderResponse(number: 1002, isBooked: true, isShipped: true));

            var result = await service.SyncOrderStatusesAsync();

            result.Updated.Should().Be(1);
            result.Skipped.Should().Be(1);

            result.Warnings.Should().ContainSingle(
                warning =>
                    warning.Contains(failingOrder.Id.ToString()) &&
                    warning.Contains("Rackbeat unavailable"));

            ResetContext();

            var orders = await Context.Orders.OrderBy(x => x.Id).ToListAsync();

            orders
                .Single(x => x.Id == failingOrder.Id)
                .Status
                .Should()
                .Be(OrderStatus.Pending);

            orders
                .Single(x => x.Id == successfulOrder.Id)
                .Status
                .Should()
                .Be(OrderStatus.Shipped);
        }

        [Fact]
        public async Task SyncOrderStatusesAsync_Should_Return_Warning_When_No_Rackbeat_Synced_Orders_Exist()
        {
            var service = GetService<IRackbeatOrderStatusSyncService>();

            var company = await CreateCompanyAsync();

            Context.Orders.Add(new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                Status = OrderStatus.Pending,
                RackbeatOrderNumber = null
            });

            await Context.SaveChangesAsync();

            var result = await service.SyncOrderStatusesAsync();

            result.Updated.Should().Be(0);
            result.Skipped.Should().Be(0);

            result.Warnings.Should().ContainSingle("No Rackbeat-synced orders found.");

            RackbeatClientMock.Verify(
                x => x.GetOrderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task SyncOrderStatusesAsync_Should_Forward_CancellationToken_To_RackbeatClient()
        {
            var service = GetService<IRackbeatOrderStatusSyncService>();

            var company = await CreateCompanyAsync();

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = "user-1",
                RackbeatOrderNumber = "RB-1001"
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            using var cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = cancellationTokenSource.Token;

            RackbeatClientMock
                .Setup(x => x.GetOrderAsync("RB-1001", cancellationToken))
                .ReturnsAsync(RackbeatTestDataFactory.CreateOrderResponse(number: 1001, isBooked: true));

            await service.SyncOrderStatusesAsync(cancellationToken);

            RackbeatClientMock.Verify(
                x => x.GetOrderAsync("RB-1001", cancellationToken),
                Times.Once);
        }
    }
}
