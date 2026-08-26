using B2BCommerceDemo.Core.DTOs.Orders;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.OrderServiceIntegrationTests
{
    public class OrderAdminIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetOrdersAdminAsync_Should_Return_All_Orders()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            Context.Orders.Add(new Order
            {
                CompanyId = company.Id,
                UserId = "user1",
                CreatedAt = DateTime.UtcNow,
                Total = 100m,
                Status = OrderStatus.Pending
            });

            Context.Orders.Add(new Order
            {
                CompanyId = company.Id,
                UserId = "user2",
                CreatedAt = DateTime.UtcNow,
                Total = 200m,
                Status = OrderStatus.Pending
            });

            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.GetOrdersAdminAsync(new OrderQueryParameters
            {
                Page = 1,
                PageSize = 10
            });

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetOrdersAdminAsync_Should_Filter_By_CompanyId()
        {
            var company1 = new Company { Name = "C1", Status = CompanyStatus.Active };
            var company2 = new Company { Name = "C2", Status = CompanyStatus.Active };

            Context.Companies.AddRange(company1, company2);
            await Context.SaveChangesAsync();

            Context.Orders.Add(new Order
            {
                CompanyId = company1.Id,
                UserId = "u1",
                CreatedAt = DateTime.UtcNow,
                Total = 100m
            });

            Context.Orders.Add(new Order
            {
                CompanyId = company2.Id,
                UserId = "u2",
                CreatedAt = DateTime.UtcNow,
                Total = 200m
            });

            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.GetOrdersAdminAsync(new OrderQueryParameters
            {
                CompanyId = company1.Id,
                Page = 1,
                PageSize = 10
            });

            result.Items.Should().HaveCount(1);
            result.Items.Single().CompanyId.Should().Be(company1.Id);
        }

        [Fact]
        public async Task GetOrdersAdminAsync_Should_Filter_By_Status()
        {
            var company = new Company { Name = "C1", Status = CompanyStatus.Active };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            Context.Orders.Add(new Order
            {
                CompanyId = company.Id,
                UserId = "u1",
                CreatedAt = DateTime.UtcNow,
                Total = 100m,
                Status = OrderStatus.Pending
            });

            Context.Orders.Add(new Order
            {
                CompanyId = company.Id,
                UserId = "u2",
                CreatedAt = DateTime.UtcNow,
                Total = 200m,
                Status = OrderStatus.Completed
            });

            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.GetOrdersAdminAsync(new OrderQueryParameters
            {
                Status = "Completed",
                Page = 1,
                PageSize = 10
            });

            result.Items.Should().HaveCount(1);
            result.Items.Single().Status.Should().Be("Completed");
        }

        [Fact]
        public async Task GetOrdersAdminAsync_Should_Paginate()
        {
            var company = new Company { Name = "C1", Status = CompanyStatus.Active };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            for (int i = 0; i < 5; i++)
            {
                Context.Orders.Add(new Order
                {
                    CompanyId = company.Id,
                    UserId = "u1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                    Total = i * 10
                });
            }

            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.GetOrdersAdminAsync(new OrderQueryParameters
            {
                Page = 1,
                PageSize = 2
            });

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(5);
        }

        [Fact]
        public async Task GetOrderByIdAdminAsync_Should_Return_Order()
        {
            var order = new Order
            {
                CompanyId = 1,
                UserId = "u1",
                CreatedAt = DateTime.UtcNow,
                Total = 123m,
                Items = new List<OrderItem>()
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.GetOrderByIdAdminAsync(order.Id);

            result.Should().NotBeNull();
            result.Total.Should().Be(123m);
        }

        [Fact]
        public async Task GetOrderByIdAdminAsync_Should_Throw_When_Not_Found()
        {
            var service = GetService<OrderService>();

            var act = () => service.GetOrderByIdAdminAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Order not found");
        }
    }
}

