using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests
{
    public class UpdateStatusTests : OrderServiceTestBase
    {
        [Fact]
        public async Task UpdateStatusAsync_Should_Update_Status()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.UpdateStatusAsync(
                1,
                OrderStatus.Confirmed);

            result.Status.Should().Be("Confirmed");
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Throw_When_Order_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateStatusAsync(
                    999,
                    OrderStatus.Confirmed);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Order not found");
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Throw_When_Order_Is_Cancelled()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Cancelled
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateStatusAsync(
                    1,
                    OrderStatus.Completed);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot change status of cancelled order");
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Publish_Status_Changed_Event()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending
            });

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.UpdateStatusAsync(
                1,
                OrderStatus.Confirmed);

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<OrderStatusChangedEvent>(e =>
                        e.OrderId == 1 &&
                        e.CompanyId == 1 &&
                        e.OldStatus == "Pending" &&
                        e.NewStatus == "Confirmed")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Save_Status_To_Database()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.UpdateStatusAsync(
                1,
                OrderStatus.Confirmed);

            var order = await context.Orders.FindAsync(1);

            order!.Status.Should().Be(
                OrderStatus.Confirmed);
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Publish_OrderConfirmedEvent()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending
            });

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.UpdateStatusAsync(
                1,
                OrderStatus.Confirmed);

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<OrderConfirmedEvent>(e =>
                        e.OrderId == 1 &&
                        e.CompanyId == 1)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Publish_OrderProcessingEvent()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Confirmed
            });

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.UpdateStatusAsync(
                1,
                OrderStatus.Processing);

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<OrderProcessingEvent>(e =>
                        e.OrderId == 1 &&
                        e.CompanyId == 1)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Publish_OrderShippedEvent()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Processing
            });

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.UpdateStatusAsync(
                1,
                OrderStatus.Shipped);

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<OrderShippedEvent>(e =>
                        e.OrderId == 1 &&
                        e.CompanyId == 1)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Publish_OrderCompletedEvent()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Shipped
            });

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.UpdateStatusAsync(
                1,
                OrderStatus.Completed);

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<OrderCompletedEvent>(e =>
                        e.OrderId == 1 &&
                        e.CompanyId == 1)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Publish_OrderCancelledEvent()
        {
            var context = CreateContext();

            context.Orders.Add(new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending
            });

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.UpdateStatusAsync(
                1,
                OrderStatus.Cancelled);

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<OrderCancelledEvent>(e =>
                        e.OrderId == 1 &&
                        e.CompanyId == 1)),
                Times.Once);
        }
    }
}
