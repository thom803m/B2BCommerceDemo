using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductImageServiceIntegrationTests
{
    public class SetPrimaryIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task SetPrimaryAsync_Should_Update_Primary_Image()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var img1 = await service.UploadFromUrlAsync(product.Id, "https://test.com/1.jpg");
            var img2 = await service.UploadFromUrlAsync(product.Id, "https://test.com/2.jpg");

            await service.SetPrimaryAsync(product.Id, img2!.Id);

            var images = Context.ProductImages.ToList();

            images.Single(x => x.Id == img1!.Id).IsPrimary.Should().BeFalse();
            images.Single(x => x.Id == img2.Id).IsPrimary.Should().BeTrue();
        }

        [Fact]
        public async Task SetPrimaryAsync_Should_Throw_When_Image_Not_Found()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var act = () => service.SetPrimaryAsync(product.Id, 999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Image with id 999 was not found for product *");
        }
    }
}

