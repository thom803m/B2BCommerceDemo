using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.OrderServiceIntegrationTests
{
    public class OrderStatusIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdateStatusAsync_Should_Update_Status_And_Return_Order()
        {
            var service = GetService<OrderService>();

            var order = new Order
            {
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Sku = "SKU1",
                        ProductName = "Test",
                        Quantity = 1,
                        UnitPrice = 100
                    }
                }
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var result = await service.UpdateStatusAsync(
                order.Id,
                OrderStatus.Confirmed);

            result.Status.Should().Be(OrderStatus.Confirmed.ToString());

            var updated = await Context.Orders.FindAsync(order.Id);
            updated!.Status.Should().Be(OrderStatus.Confirmed);
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Throw_When_Order_Not_Found()
        {
            var service = GetService<OrderService>();

            var act = () => service.UpdateStatusAsync(
                orderId: 999,
                OrderStatus.Confirmed);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Order not found");
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Throw_When_Order_Is_Cancelled()
        {
            var service = GetService<OrderService>();

            var order = new Order
            {
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Cancelled,
                CreatedAt = DateTime.UtcNow
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var act = () => service.UpdateStatusAsync(
                order.Id,
                OrderStatus.Processing);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot change status of cancelled order");
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Throw_When_Invalid_Status_Transition()
        {
            var service = GetService<OrderService>();

            var order = new Order
            {
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var act = () => service.UpdateStatusAsync(
                order.Id,
                OrderStatus.Pending);

            await act.Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateStatusAsync_Should_Progress_Through_Full_Status_Flow()
        {
            var service = GetService<OrderService>();

            var order = new Order
            {
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var confirmed = await service.UpdateStatusAsync(
                order.Id,
                OrderStatus.Confirmed);

            confirmed.Status.Should().Be(OrderStatus.Confirmed.ToString());

            var processing = await service.UpdateStatusAsync(
                order.Id,
                OrderStatus.Processing);

            processing.Status.Should().Be(OrderStatus.Processing.ToString());

            var shipped = await service.UpdateStatusAsync(
                order.Id,
                OrderStatus.Shipped);

            shipped.Status.Should().Be(OrderStatus.Shipped.ToString());

            var updated = await Context.Orders.FindAsync(order.Id);

            updated!.Status.Should().Be(OrderStatus.Shipped);
        }
    }
}

