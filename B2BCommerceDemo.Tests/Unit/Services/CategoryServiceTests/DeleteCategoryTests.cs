using B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests
{
    public class DeleteCategoryTests : CategoryServiceTestBase
    {
        [Fact]
        public async Task DeleteCategoryAsync_Should_Delete_Category()
        {
            var context = CreateContext();

            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.DeleteCategoryAsync(1);

            context.Categories.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteCategoryAsync_Should_Throw_When_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () => await service.DeleteCategoryAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Category not found");
        }
    }
}
