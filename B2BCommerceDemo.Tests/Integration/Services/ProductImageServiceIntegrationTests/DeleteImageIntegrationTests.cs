using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductImageServiceIntegrationTests
{
    public class DeleteImageIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task DeleteImageAsync_Should_Remove_Image()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var img1 = await service.UploadFromUrlAsync(product.Id, "https://test.com/1.jpg");
            var img2 = await service.UploadFromUrlAsync(product.Id, "https://test.com/2.jpg");

            await service.DeleteImageAsync(product.Id, img2!.Id);

            Context.ProductImages.Should().HaveCount(1);
        }

        [Fact]
        public async Task DeleteImageAsync_Should_Set_New_Primary_When_Primary_Is_Deleted()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var img1 = await service.UploadFromUrlAsync(product.Id, "https://test.com/1.jpg");
            var img2 = await service.UploadFromUrlAsync(product.Id, "https://test.com/2.jpg");

            await service.DeleteImageAsync(product.Id, img1!.Id);

            var remaining = Context.ProductImages.Single();

            remaining.IsPrimary.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteImageAsync_Should_Throw_When_Image_Not_Found()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var act = () => service.DeleteImageAsync(product.Id, 999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>();
        }
    }
}

