using B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests
{
    public class DeleteBrandTests : BrandServiceTestBase
    {
        [Fact]
        public async Task DeleteBrandAsync_Should_Remove_Brand()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.DeleteBrandAsync(1);

            context.Brands.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteBrandAsync_Should_Throw_When_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () => await service.DeleteBrandAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Brand not found");
        }
    }
}
