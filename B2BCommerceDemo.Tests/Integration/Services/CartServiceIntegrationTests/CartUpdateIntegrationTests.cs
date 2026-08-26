using B2BCommerceDemo.Core.DTOs.Carts;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Tests.Integration.Services.CartServiceIntegrationTests
{
    public class CartUpdateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdateItemAsync_Should_Update_Quantity()
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
                        Quantity = 1,
                        UnitPrice = 100m
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var item = cart.Items.Single();

            var service = GetService<CartService>();

            var dto = new UpdateCartItemDto
            {
                Quantity = 5
            };

            var result = await service.UpdateItemAsync(company.Id, user.Id, item.Id, dto);

            result.Items.Single().Quantity.Should().Be(5);
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Cart_Not_Found()
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

            var service = GetService<CartService>();

            var dto = new UpdateCartItemDto
            {
                Quantity = 2
            };

            var act = () => service.UpdateItemAsync(company.Id, user.Id, 1, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart not found");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Item_Not_Found()
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
                Items = new List<CartItem>()
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var service = GetService<CartService>();

            var dto = new UpdateCartItemDto
            {
                Quantity = 2
            };

            var act = () => service.UpdateItemAsync(company.Id, user.Id, 999, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart item not found");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Product_Not_Found()
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
                        Quantity = 1,
                        UnitPrice = 100m
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var itemId = Context.CartItems
                .Where(x => x.CartId == cart.Id)
                .Select(x => x.Id)
                .Single();

            await Context.Database.ExecuteSqlRawAsync(
                "DELETE FROM Products WHERE Id = {0}",
                product.Id);

            var service = GetService<CartService>();

            var dto = new UpdateCartItemDto
            {
                Quantity = 2
            };

            var act = () => service.UpdateItemAsync(company.Id, user.Id, itemId, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Product_Is_Inactive()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(isActive: false);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 1,
                        UnitPrice = 100m
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var item = cart.Items.Single();

            var service = GetService<CartService>();

            var dto = new UpdateCartItemDto
            {
                Quantity = 2
            };

            var act = () => service.UpdateItemAsync(company.Id, user.Id, item.Id, dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Product unavailable");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Not_Enough_Stock()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(stock: 3);

            var cart = new Cart
            {
                CompanyId = company.Id,
                UserId = user.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = 1,
                        UnitPrice = 100m
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var item = cart.Items.Single();

            var service = GetService<CartService>();

            var dto = new UpdateCartItemDto
            {
                Quantity = 10
            };

            var act = () => service.UpdateItemAsync(company.Id, user.Id, item.Id, dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Not enough stock");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Quantity_Is_Invalid()
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
                        Quantity = 1,
                        UnitPrice = 100m
                    }
                }
            };

            Context.Carts.Add(cart);
            await Context.SaveChangesAsync();

            var item = cart.Items.Single();

            var service = GetService<CartService>();

            var dto = new UpdateCartItemDto
            {
                Quantity = 0
            };

            var act = () => service.UpdateItemAsync(company.Id, user.Id, item.Id, dto);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than 0");
        }
    }
}

