using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CartServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CartServiceTests
{
    public class RemoveItemTests : CartServiceTestBase
    {
        [Fact]
        public async Task RemoveItemAsync_Should_Remove_Item()
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
                Quantity = 1
            }
        }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.RemoveItemAsync(
                1,
                "user1",
                10);

            context.CartItems.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveItemAsync_Should_Return_Updated_Cart()
        {
            var context = CreateContext();

            context.Products.AddRange(
                CreateProduct(1),
                CreateProduct(2));

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
            },
            new CartItem
            {
                Id = 11,
                ProductId = 2,
                Quantity = 1
            }
        }
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.RemoveItemAsync(
                1,
                "user1",
                10);

            result.Items.Should().HaveCount(1);
            result.Items.Single().ProductId.Should().Be(2);
        }

        [Fact]
        public async Task RemoveItemAsync_Should_Throw_When_Cart_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.RemoveItemAsync(
                    1,
                    "user1",
                    10);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart not found");
        }

        [Fact]
        public async Task RemoveItemAsync_Should_Throw_When_Item_Not_Found()
        {
            var context = CreateContext();

            context.Carts.Add(
                CreateCart(1, "user1"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.RemoveItemAsync(
                    1,
                    "user1",
                    999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Cart item not found");
        }

        [Fact]
        public async Task RemoveItemAsync_Should_Validate_Company_Is_Active()
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

            await service.RemoveItemAsync(
                1,
                "user1",
                10);

            validator.Verify(
                x => x.ValidateCompanyActiveAsync(1),
                Times.AtLeastOnce);
        }
    }
}

