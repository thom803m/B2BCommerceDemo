using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.ProductImageServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductImageServiceTests
{
    public class UploadFromUrlTests : ProductImageServiceTestBase
    {
        [Fact]
        public async Task Upload_Should_Create_Image()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct());
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.UploadFromUrlAsync(1, " https://test.com/img.jpg ");

            result.Should().NotBeNull();
            result!.Url.Should().Be("https://test.com/img.jpg");
            result.IsPrimary.Should().BeTrue();
        }

        [Fact]
        public async Task Upload_Should_Return_Existing_Image_If_Duplicate()
        {
            var context = CreateContext();

            var product = CreateProduct();
            var image = CreateImage(url: "https://test.com/img.jpg");

            product.Images = new List<ProductImage> { image };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.UploadFromUrlAsync(1, "https://test.com/img.jpg");

            result.Should().NotBeNull();
            result!.Id.Should().Be(image.Id);
        }

        [Fact]
        public async Task Upload_Should_Throw_When_Product_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UploadFromUrlAsync(99, "url");

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product with id 99 was not found.");
        }

        [Fact]
        public async Task Upload_Should_Throw_When_Url_Empty()
        {
            var context = CreateContext();

            context.Products.Add(CreateProduct());
            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UploadFromUrlAsync(1, "   ");

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Image URL is required");
        }
    }
}
