using B2BCommerceDemo.Core.DTOs.Categories;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CategoryServiceIntegrationTests
{
    public class CategoryUpdateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdateCategory_Should_Update_Name()
        {
            var service = GetService<CategoryService>();

            var created = await service.CreateCategoryAsync(new CreateCategoryDto
            {
                Name = "Electronics"
            });

            var updated = await service.UpdateCategoryAsync(
                created.Id,
                new UpdateCategoryDto
                {
                    Name = "Computers"
                });

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Computers");

            var fromDb =
                await service.GetCategoryByIdAsync(created.Id);

            fromDb!.Name.Should().Be("Computers");
        }

        [Fact]
        public async Task UpdateCategory_Should_Throw_When_Name_Already_Exists()
        {
            var service = GetService<CategoryService>();

            await service.CreateCategoryAsync(new CreateCategoryDto
            {
                Name = "Electronics"
            });

            var category = await service.CreateCategoryAsync(new CreateCategoryDto
            {
                Name = "Computers"
            });

            Func<Task> act = async () =>
                await service.UpdateCategoryAsync(
                    category.Id,
                    new UpdateCategoryDto
                    {
                        Name = "Electronics"
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Category 'Electronics' already exists");
        }
    }
}

