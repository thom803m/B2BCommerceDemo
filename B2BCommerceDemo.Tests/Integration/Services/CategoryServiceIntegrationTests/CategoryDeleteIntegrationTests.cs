using B2BCommerceDemo.Core.DTOs.Categories;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CategoryServiceIntegrationTests
{
    public class CategoryDeleteIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task DeleteCategory_Should_Remove_Category()
        {
            var service = GetService<CategoryService>();

            var created = await service.CreateCategoryAsync(new CreateCategoryDto
            {
                Name = "Electronics"
            });

            await service.DeleteCategoryAsync(created.Id);

            Func<Task> act = async () =>
                await service.GetCategoryByIdAsync(created.Id);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Category not found");
        }
    }
}

