using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests
{
    public class GetBrandTests : BrandServiceTestBase
    {
        [Fact]
        public async Task GetBrandsAsync_Should_Return_Ordered_List()
        {
            var context = CreateContext();

            context.Brands.AddRange(
                new Brand { Id = 1, Name = "Zebra" },
                new Brand { Id = 2, Name = "Apple" });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetBrandsAsync();

            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Apple");
            result[1].Name.Should().Be("Zebra");
        }

        [Fact]
        public async Task GetBrandByIdAsync_Should_Return_Brand()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetBrandByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Brand");
        }

        [Fact]
        public async Task GetBrandByIdAsync_Should_Throw_When_Not_Found()
        {
            var context = CreateContext();
            var service = CreateService(context);

            Func<Task> act = async () => await service.GetBrandByIdAsync(99);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Brand not found");
        }
    }
}
