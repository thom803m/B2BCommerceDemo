using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.OrderServiceIntegrationTests
{
    public class OrderReadIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetOrdersAsync_Should_Return_Empty_List_When_No_Orders()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var service = GetService<OrderService>();

            var result = await service.GetOrdersAsync(company.Id, user.Id);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetOrdersAsync_Should_Return_Orders()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Total = 200m,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        ProductName = "Test",
                        Sku = "SKU-1",
                        Quantity = 2,
                        UnitPrice = 100m
                    }
                }
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.GetOrdersAsync(company.Id, user.Id);

            result.Should().HaveCount(1);

            var first = result.Single();

            first.Total.Should().Be(200m);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Return_Order()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var order = new Order
            {
                CompanyId = company.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Total = 150m,
                Items = new List<OrderItem>()
            };

            Context.Orders.Add(order);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.GetOrderByIdAsync(company.Id, user.Id, order.Id);

            result.Should().NotBeNull();
            result.Total.Should().Be(150m);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Throw_When_Order_Not_Found()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var service = GetService<OrderService>();

            var act = () => service.GetOrderByIdAsync(company.Id, user.Id, 999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Order not found");
        }
    }
}

