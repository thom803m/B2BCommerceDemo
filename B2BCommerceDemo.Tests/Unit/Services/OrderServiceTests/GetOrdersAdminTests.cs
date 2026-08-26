using B2BCommerceDemo.Core.DTOs.Orders;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests;

public class GetOrdersAdminTests : OrderServiceTestBase
{
    [Fact]
    public async Task GetOrdersAdminAsync_Should_Return_All_Orders_When_No_Filters()
    {
        var context = CreateContext();

        context.Orders.AddRange(
            CreateOrder(1, 1, "user1"),
            CreateOrder(2, 1, "user2"));

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetOrdersAdminAsync(
            new OrderQueryParameters());

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetOrdersAdminAsync_Should_Filter_By_Company()
    {
        var context = CreateContext();

        context.Orders.AddRange(
            CreateOrder(1, 1, "user1"),
            CreateOrder(2, 2, "user1"));

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetOrdersAdminAsync(
            new OrderQueryParameters
            {
                CompanyId = 1
            });

        result.Items.Should().HaveCount(1);
        result.Items.First().CompanyId.Should().Be(1);
    }

    [Fact]
    public async Task GetOrdersAdminAsync_Should_Filter_By_Status()
    {
        var context = CreateContext();

        context.Orders.AddRange(
            new Order
            {
                Id = 1,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Pending
            },
            new Order
            {
                Id = 2,
                CompanyId = 1,
                UserId = "user1",
                Status = OrderStatus.Completed
            });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetOrdersAdminAsync(
            new OrderQueryParameters
            {
                Status = "Completed"
            });

        result.Items.Should().HaveCount(1);
        result.Items.First().Id.Should().Be(2);
    }

    [Fact]
    public async Task GetOrdersAdminAsync_Should_Order_By_CreatedAt_Descending()
    {
        var context = CreateContext();

        context.Orders.AddRange(
            new Order
            {
                Id = 1,
                CompanyId = 1,
                CreatedAt = new DateTime(2025, 1, 1)
            },
            new Order
            {
                Id = 2,
                CompanyId = 1,
                CreatedAt = new DateTime(2025, 1, 2)
            });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetOrdersAdminAsync(
            new OrderQueryParameters());

        result.Items.First().Id.Should().Be(2);
    }

    [Fact]
    public async Task GetOrdersAdminAsync_Should_Apply_Paging()
    {
        var context = CreateContext();

        context.Orders.AddRange(
            CreateOrder(1, 1, "u1"),
            CreateOrder(2, 1, "u1"),
            CreateOrder(3, 1, "u1"));

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetOrdersAdminAsync(
            new OrderQueryParameters
            {
                Page = 2,
                PageSize = 1
            });

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(3);
    }
}
