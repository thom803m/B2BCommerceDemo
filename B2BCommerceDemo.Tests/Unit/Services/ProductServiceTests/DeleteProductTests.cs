using B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductServiceTests
{
    public class DeleteProductTests : ProductServiceTestBase
    {
        [Fact]
        public async Task DeleteProductAsync_Should_Delete_Product()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct(1, 100));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.DeleteProductAsync(1);

            var product = await context.Products.FindAsync(1);

            product.Should().BeNull();
        }

        [Fact]
        public async Task DeleteProductAsync_Should_Throw_When_Product_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act =
                async () => await service.DeleteProductAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }
    }
}

