using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CartServiceIntegrationTests
{
    public class CartReadIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetCartAsync_Should_Return_Empty_Cart_When_Not_Found()
        {
            var service = GetService<CartService>();

            var company = await CreateCompanyAsync();

            var result = await service.GetCartAsync(
                companyId: 1,
                userId: "user1");

            result.Should().NotBeNull();
            result.CompanyId.Should().Be(1);
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCartAsync_Should_Return_Empty_Items_When_Cart_Has_No_Items()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(
                email: "user@test.dk",
                companyId: company.Id);

            Context.Carts.Add(new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id
            });

            await Context.SaveChangesAsync();

            var service = GetService<CartService>();

            var result = await service.GetCartAsync(company.Id, user.Id);

            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCartAsync_Should_Return_Items()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(
                email: "user@test.dk",
                companyId: company.Id);

            var product = await CreateProductAsync();

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 2,
                        UnitPrice = 100m
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<CartService>();

            var result = await service.GetCartAsync(company.Id, user.Id);

            result.Items.Should().HaveCount(1);

            var item = result.Items.Single();
            item.Quantity.Should().Be(2);
        }
    }
}

