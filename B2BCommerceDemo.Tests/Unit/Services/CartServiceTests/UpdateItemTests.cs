using B2BCommerceDemo.Core.DTOs.Carts;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CartServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CartServiceTests
{
    public class UpdateItemTests : CartServiceTestBase
    {
        [Fact]
        public async Task UpdateItemAsync_Should_Update_Quantity()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        Id = 10,
                        ProductId = 1,
                        Quantity = 1,
                        UnitPrice = 100
                    }
                }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.UpdateItemAsync(
                1,
                "user1",
                10,
                new UpdateCartItemDto
                {
                    Quantity = 5
                });

            result.Items.Single().Quantity.Should().Be(5);
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Cart_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateItemAsync(
                    1,
                    "user1",
                    10,
                    new UpdateCartItemDto
                    {
                        Quantity = 1
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart not found");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Item_Not_Found()
        {
            var context = CreateContext();

            context.Carts.Add(
                CreateCart(1, "user1"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateItemAsync(
                    1,
                    "user1",
                    999,
                    new UpdateCartItemDto
                    {
                        Quantity = 1
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart item not found");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Product_Not_Found()
        {
            var context = CreateContext();

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
        {
            new CartItem
            {
                Id = 10,
                ProductId = 999,
                Quantity = 1
            }
        }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateItemAsync(
                    1,
                    "user1",
                    10,
                    new UpdateCartItemDto
                    {
                        Quantity = 1
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Product_Is_Inactive()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1, active: false));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
        {
            new CartItem
            {
                Id = 10,
                ProductId = 1,
                Quantity = 1
            }
        }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateItemAsync(
                    1,
                    "user1",
                    10,
                    new UpdateCartItemDto
                    {
                        Quantity = 2
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Product unavailable");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Quantity_Is_Zero()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateItemAsync(
                    1,
                    "user1",
                    10,
                    new UpdateCartItemDto
                    {
                        Quantity = 0
                    });

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Quantity must be greater than 0");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Throw_When_Stock_Is_Insufficient()
        {
            var context = CreateContext();

            context.Products.Add(
                CreateProduct(1, stock: 2));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
        {
            new CartItem
            {
                Id = 10,
                ProductId = 1,
                Quantity = 1
            }
        }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateItemAsync(
                    1,
                    "user1",
                    10,
                    new UpdateCartItemDto
                    {
                        Quantity = 5
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Not enough stock");
        }

        [Fact]
        public async Task UpdateItemAsync_Should_Validate_Company_Is_Active()
        {
            var context = CreateContext();

            var validator = CreateValidator();

            context.Products.Add(CreateProduct(1));

            context.Carts.Add(new Cart
            {
                CompanyId = 1,
                UserId = "user1",
                Items = new List<CartItem>
        {
            new CartItem
            {
                Id = 10,
                ProductId = 1,
                Quantity = 1
            }
        }
            });

            await context.SaveChangesAsync();

            var service = CreateService(
                context,
                validator: validator);

            await service.UpdateItemAsync(
                1,
                "user1",
                10,
                new UpdateCartItemDto
                {
                    Quantity = 2
                });

            validator.Verify(
                x => x.ValidateCompanyActiveAsync(1),
                Times.AtLeastOnce);
        }
    }
}

