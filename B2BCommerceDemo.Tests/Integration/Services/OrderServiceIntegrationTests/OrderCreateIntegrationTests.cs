using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.OrderServiceIntegrationTests
{
    public class OrderCreateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateFromCartAsync_Should_Create_Order_From_Cart()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(stock: 10);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 2
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.CreateFromCartAsync(
                company.Id,
                user.Id,
                "idem-1");

            result.WasCreated.Should().BeTrue();
            result.Order.Should().NotBeNull();
            result.Order.Items.Should().HaveCount(1);

            Context.Orders.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Return_Existing_Order_When_Idempotency_Key_Exists()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(stock: 10);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            await service.CreateFromCartAsync(company.Id, user.Id, "idem-123");

            var second = await service.CreateFromCartAsync(company.Id, user.Id, "idem-123");

            second.WasCreated.Should().BeFalse();
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Throw_When_Cart_Is_Empty()
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

            var act = () => service.CreateFromCartAsync(
                company.Id,
                user.Id,
                "idem-1");

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Cart is empty");
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Decrease_Product_Stock()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(stock: 10);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 3
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            await service.CreateFromCartAsync(company.Id, user.Id, "idem-1");

            var updatedProduct = Context.Products.Single();

            updatedProduct.AvailableStock.Should().Be(7);
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Clear_Cart()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(stock: 10);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 1
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            await service.CreateFromCartAsync(company.Id, user.Id, "idem-1");

            Context.CartItems.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateFromCartAsync_Should_Calculate_Total()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(stock: 10);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 2
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<OrderService>();

            var result = await service.CreateFromCartAsync(company.Id, user.Id, "idem-1");

            result.Order.Total.Should().BeGreaterThan(0);
        }
    }
}

