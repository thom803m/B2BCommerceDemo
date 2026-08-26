using FluentAssertions;
using B2BCommerceDemo.Tests.Unit.Services.ProductImageServiceTests.Shared;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductImageServiceTests
{
    public class DeleteImageTests : ProductImageServiceTestBase
    {
        [Fact]
        public async Task Delete_Should_Remove_Image()
        {
            var context = CreateContext();

            var img = CreateImage(1, 1);
            img.IsPrimary = true;

            context.ProductImages.Add(img);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.DeleteImageAsync(1, 1);

            context.ProductImages.Should().BeEmpty();
        }

        [Fact]
        public async Task Delete_Should_Set_Next_Image_As_Primary_When_Primary_Is_Deleted()
        {
            var context = CreateContext();

            var img1 = CreateImage(1, 1);
            img1.IsPrimary = true;

            var img2 = CreateImage(2, 1);
            img2.IsPrimary = true;

            context.ProductImages.AddRange(img1, img2);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.DeleteImageAsync(1, 1);

            var remaining = context.ProductImages.First();

            remaining.IsPrimary.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_Should_Throw_When_Image_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.DeleteImageAsync(1, 99);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Image with id 99 was not found.");
        }
    }
}
