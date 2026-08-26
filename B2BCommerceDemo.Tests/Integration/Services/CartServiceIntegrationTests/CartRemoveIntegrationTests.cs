using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CartServiceIntegrationTests
{
    public class CartRemoveIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task RemoveItemAsync_Should_Remove_Item_From_Cart()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

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

            var item = cart.Items.Single();

            var service = GetService<CartService>();

            var result = await service.RemoveItemAsync(company.Id, user.Id, item.Id);

            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveItemAsync_Should_Throw_When_Cart_Not_Found()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var service = GetService<CartService>();

            var act = () => service.RemoveItemAsync(company.Id, user.Id, 1);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart not found");
        }

        [Fact]
        public async Task RemoveItemAsync_Should_Throw_When_Item_Not_Found()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>()
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<CartService>();

            var act = () => service.RemoveItemAsync(company.Id, user.Id, 999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart item not found");
        }
    }
}

