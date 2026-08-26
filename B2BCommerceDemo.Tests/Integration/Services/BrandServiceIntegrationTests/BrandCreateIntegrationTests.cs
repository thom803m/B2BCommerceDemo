using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.BrandServiceIntegrationTests
{
    public class BrandCreateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateBrand_Should_Create_Brand()
        {
            var service = GetService<BrandService>();

            var created = await service.CreateBrandAsync(new CreateBrandDto
            {
                Name = "Apple"
            });

            var result = await service.GetBrandByIdAsync(created.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Apple");
        }

        [Fact]
        public async Task CreateBrand_Should_Throw_When_Name_Already_Exists()
        {
            var service = GetService<BrandService>();

            await service.CreateBrandAsync(new CreateBrandDto
            {
                Name = "Apple"
            });

            Func<Task> act = async () =>
                await service.CreateBrandAsync(new CreateBrandDto
                {
                    Name = "apple"
                });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Brand 'apple' already exists");
        }
    }
}

