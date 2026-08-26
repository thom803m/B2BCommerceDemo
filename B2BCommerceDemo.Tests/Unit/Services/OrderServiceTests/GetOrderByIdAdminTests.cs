using B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests;

public class GetOrderByIdAdminTests : OrderServiceTestBase
{
    [Fact]
    public async Task GetOrderByIdAdminAsync_Should_Return_Order()
    {
        var context = CreateContext();

        context.Orders.Add(
            CreateOrder(1, 1, "user1"));

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetOrderByIdAdminAsync(1);

        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetOrderByIdAdminAsync_Should_Throw_When_Order_Not_Found()
    {
        var context = CreateContext();

        var service = CreateService(context);

        Func<Task> act = async () =>
            await service.GetOrderByIdAdminAsync(999);

        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Order not found");
    }
}
