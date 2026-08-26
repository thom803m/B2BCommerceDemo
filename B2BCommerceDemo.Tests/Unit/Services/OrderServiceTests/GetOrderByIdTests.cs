using B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests
{
    public class GetOrderByIdTests : OrderServiceTestBase
    {
        [Fact]
        public async Task GetOrderByIdAsync_Should_Return_Order()
        {
            var context = CreateContext();

            context.Orders.Add(CreateOrder(1, 1, "user1"));
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetOrderByIdAsync(1, "user1", 1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Throw_When_Order_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.GetOrderByIdAsync(1, "user1", 999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Order not found");
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Validate_Company_Is_Active()
        {
            var context = CreateContext();

            context.Orders.Add(CreateOrder(1, 1, "user1"));
            await context.SaveChangesAsync();

            var validator = CreateValidator();

            var service = CreateService(context, validator);

            await service.GetOrderByIdAsync(1, "user1", 1);

            validator.Verify(
                x => x.ValidateCompanyActiveAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Not_Return_Order_From_Other_User()
        {
            var context = CreateContext();

            context.Orders.Add(CreateOrder(1, 1, "user2"));
            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.GetOrderByIdAsync(1, "user1", 1);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Order not found");
        }
    }
}
