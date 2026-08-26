using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.BrandServiceIntegrationTests
{
    public class BrandUpdateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdateBrand_Should_Update_Name()
        {
            var service = GetService<BrandService>();

            var created = await service.CreateBrandAsync(new CreateBrandDto
            {
                Name = "Apple"
            });

            var updated = await service.UpdateBrandAsync(
                created.Id,
                new UpdateBrandDto
                {
                    Name = "Samsung"
                });

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Samsung");

            var fromDb = await service.GetBrandByIdAsync(created.Id);

            fromDb!.Name.Should().Be("Samsung");
        }

        [Fact]
        public async Task UpdateBrand_Should_Throw_When_Name_Already_Exists()
        {
            var service = GetService<BrandService>();

            await service.CreateBrandAsync(new CreateBrandDto
            {
                Name = "Apple"
            });

            var brand = await service.CreateBrandAsync(new CreateBrandDto
            {
                Name = "Samsung"
            });

            Func<Task> act = async () =>
                await service.UpdateBrandAsync(
                    brand.Id,
                    new UpdateBrandDto
                    {
                        Name = "Apple"
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Brand 'Apple' already exists");
        }
    }
}

