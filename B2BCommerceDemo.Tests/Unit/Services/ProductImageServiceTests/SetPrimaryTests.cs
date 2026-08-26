using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.ProductImageServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductImageServiceTests
{
    public class SetPrimaryTests : ProductImageServiceTestBase
    {
        [Fact]
        public async Task SetPrimary_Should_Update_Primary_Image()
        {
            var context = CreateContext();

            var product = CreateProduct();

            var img1 = CreateImage(1, 1, "url1");
            img1.IsPrimary = true;

            var img2 = CreateImage(2, 1, "url2");

            product.Images = new List<ProductImage> { img1, img2 };

            context.Products.Add(product);
            context.ProductImages.AddRange(img1, img2);

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.SetPrimaryAsync(1, 2);

            var images = context.ProductImages.ToList();

            images.First(i => i.Id == 1).IsPrimary.Should().BeFalse();
            images.First(i => i.Id == 2).IsPrimary.Should().BeTrue();
        }

        [Fact]
        public async Task SetPrimary_Should_Throw_When_Image_Not_Found()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct());
            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.SetPrimaryAsync(1, 999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Image with id 999 was not found for product 1.");
        }
    }
}
