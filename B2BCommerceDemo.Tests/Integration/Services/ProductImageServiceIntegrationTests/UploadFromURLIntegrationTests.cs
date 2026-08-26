using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductImageServiceIntegrationTests
{
    public class UploadFromURLIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UploadFromUrlAsync_Should_Create_Image_And_Set_As_Primary_When_First_Image()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var result = await service.UploadFromUrlAsync(product.Id, "https://test.com/img1.jpg");

            result.Should().NotBeNull();
            result!.IsPrimary.Should().BeTrue();
            result.Source.Should().Be("Manual");
            result.ExternalId.Should().BeNull();
            result.LastSynced.Should().BeNull();

            Context.ProductImages.Should().ContainSingle();
        }

        [Fact]
        public async Task UploadFromUrlAsync_Should_Not_Set_Primary_When_Product_Already_Has_Image()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            await service.UploadFromUrlAsync(product.Id, "https://test.com/img1.jpg");

            var result = await service.UploadFromUrlAsync(product.Id, "https://test.com/img2.jpg");

            result!.IsPrimary.Should().BeFalse();
            Context.ProductImages.Should().HaveCount(2);
        }

        [Fact]
        public async Task UploadFromUrlAsync_Should_Return_Existing_Image_When_Url_Already_Exists()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var first = await service.UploadFromUrlAsync(product.Id, "https://test.com/img1.jpg");
            var second = await service.UploadFromUrlAsync(product.Id, "https://test.com/img1.jpg");

            second.Should().Be(first);
            Context.ProductImages.Should().HaveCount(1);
        }

        [Fact]
        public async Task UploadFromUrlAsync_Should_Throw_When_Url_Is_Empty()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var act = () => service.UploadFromUrlAsync(product.Id, "");

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Image URL is required");
        }

        [Fact]
        public async Task UploadFromUrlAsync_Should_Trim_Image_Url()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var result = await service.UploadFromUrlAsync(
                product.Id,
                "  https://test.com/img1.jpg  ");

            result!.Url.Should().Be("https://test.com/img1.jpg");
        }

        [Fact]
        public async Task UploadFromUrlAsync_Should_Set_Rackbeat_Metadata_When_Source_Is_Rackbeat()
        {
            var product = await CreateProductAsync();
            var service = GetService<ProductImageService>();

            var result = await service.UploadFromUrlAsync(
                product.Id,
                "https://rackbeat.dk/image.jpg",
                source: "Rackbeat");

            result.Should().NotBeNull();
            result!.Source.Should().Be("Rackbeat");
            result.ExternalId.Should().Be("https://rackbeat.dk/image.jpg");
            result.LastSynced.Should().NotBeNull();
        }
    }
}

