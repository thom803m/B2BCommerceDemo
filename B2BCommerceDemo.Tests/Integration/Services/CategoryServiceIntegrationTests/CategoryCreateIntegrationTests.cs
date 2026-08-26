using B2BCommerceDemo.Core.DTOs.Categories;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CategoryServiceIntegrationTests
{
    public class CategoryCreateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateCategory_Should_Create_Category()
        {
            var service = GetService<CategoryService>();

            var created = await service.CreateCategoryAsync(new CreateCategoryDto
            {
                Name = "Electronics"
            });

            var result = await service.GetCategoryByIdAsync(created.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Electronics");
        }

        [Fact]
        public async Task CreateCategory_Should_Throw_When_Name_Already_Exists()
        {
            var service = GetService<CategoryService>();

            await service.CreateCategoryAsync(new CreateCategoryDto
            {
                Name = "Electronics"
            });

            Func<Task> act = async () =>
                await service.CreateCategoryAsync(new CreateCategoryDto
                {
                    Name = "electronics"
                });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Category 'electronics' already exists");
        }
    }
}

