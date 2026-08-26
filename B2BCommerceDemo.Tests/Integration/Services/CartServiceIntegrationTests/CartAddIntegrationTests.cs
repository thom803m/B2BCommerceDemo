using B2BCommerceDemo.Core.DTOs.Carts;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CartServiceIntegrationTests
{
    public class CartAddIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task AddItemAsync_Should_Create_Cart_And_Add_Item()
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

            var service = GetService<CartService>();

            var dto = new CreateCartItemDto
            {
                ProductId = product.Id,
                Quantity = 2
            };

            var result = await service.AddItemAsync(company.Id, user.Id, dto);

            result.Items.Should().HaveCount(1);

            var item = result.Items.Single();
            item.ProductId.Should().Be(product.Id);
            item.Quantity.Should().Be(2);
        }

        [Fact]
        public async Task AddItemAsync_Should_Increase_Quantity_When_Item_Already_Exists()
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

            var service = GetService<CartService>();

            var dto = new CreateCartItemDto
            {
                ProductId = product.Id,
                Quantity = 2
            };

            var result = await service.AddItemAsync(company.Id, user.Id, dto);

            result.Items.Single().Quantity.Should().Be(3);
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Product_Not_Found()
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

            var dto = new CreateCartItemDto
            {
                ProductId = 999,
                Quantity = 1
            };

            var act = () => service.AddItemAsync(company.Id, user.Id, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Product_Is_Inactive()
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

            var service = GetService<CartService>();

            var dto = new CreateCartItemDto
            {
                ProductId = product.Id,
                Quantity = 1
            };

            var act = () => service.AddItemAsync(company.Id, user.Id, dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Product unavailable");
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Not_Enough_Stock()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            var user = await CreateUserAsync(companyId: company.Id);

            var product = await CreateProductAsync(stock: 1);

            var service = GetService<CartService>();

            var dto = new CreateCartItemDto
            {
                ProductId = product.Id,
                Quantity = 5
            };

            var act = () => service.AddItemAsync(company.Id, user.Id, dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Not enough stock");
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_Quantity_Is_Invalid()
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

            var dto = new CreateCartItemDto
            {
                ProductId = product.Id,
                Quantity = 0
            };

            var act = () => service.AddItemAsync(company.Id, user.Id, dto);

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than 0");
        }
    }
}

