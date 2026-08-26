using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.OrderServiceTests
{
    public class GetOrdersTests : OrderServiceTestBase
    {
        [Fact]
        public async Task GetOrdersAsync_Should_Return_User_Orders()
        {
            var context = CreateContext();

            context.Orders.AddRange(
                CreateOrder(1, 1, "user1"),
                CreateOrder(2, 1, "user1"),
                CreateOrder(3, 1, "user2"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetOrdersAsync(1, "user1");

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrdersAsync_Should_Return_Empty_List_When_No_Orders()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var result = await service.GetOrdersAsync(1, "user1");

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetOrdersAsync_Should_Order_By_CreatedAt_Descending()
        {
            var context = CreateContext();

            context.Orders.AddRange(
                new Order
                {
                    Id = 1,
                    CompanyId = 1,
                    UserId = "user1",
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new Order
                {
                    Id = 2,
                    CompanyId = 1,
                    UserId = "user1",
                    CreatedAt = new DateTime(2025, 1, 2)
                });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetOrdersAsync(1, "user1");

            result[0].Id.Should().Be(2);
            result[1].Id.Should().Be(1);
        }

        [Fact]
        public async Task GetOrdersAsync_Should_Validate_Company_Is_Active()
        {
            var context = CreateContext();

            var validator = CreateValidator();

            var service = CreateService(context, validator);

            await service.GetOrdersAsync(1, "user1");

            validator.Verify(
                x => x.ValidateCompanyActiveAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task GetOrdersAsync_Should_Only_Return_Orders_For_Company()
        {
            var context = CreateContext();

            context.Orders.AddRange(
                CreateOrder(1, 1, "user1"),
                CreateOrder(2, 2, "user1"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetOrdersAsync(1, "user1");

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1);
        }
    }
}
