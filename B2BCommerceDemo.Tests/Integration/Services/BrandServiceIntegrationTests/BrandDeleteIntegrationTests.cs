using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions; 

namespace B2BCommerceDemo.Tests.Integration.Services.BrandServiceIntegrationTests
{
    public class BrandDeleteIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task DeleteBrand_Should_Remove_Brand()
        {
            var service = GetService<BrandService>();

            var created = await service.CreateBrandAsync(new CreateBrandDto
            {
                Name = "Apple"
            });

            await service.DeleteBrandAsync(created.Id);

            Func<Task> act = async () =>
                await service.GetBrandByIdAsync(created.Id);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Brand not found");
        }
    }
}

