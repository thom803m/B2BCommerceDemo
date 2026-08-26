using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests
{
    public class GetCategoriesTests : CategoryServiceTestBase
    {
        [Fact]
        public async Task GetCategoriesAsync_Should_Return_All_Categories_Ordered()
        {
            var context = CreateContext();

            context.Categories.AddRange(
                new Category { Name = "Printers" },
                new Category { Name = "Mobiles" });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetCategoriesAsync();

            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Mobiles");
            result[1].Name.Should().Be("Printers");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_Should_Return_Category()
        {
            var context = CreateContext();

            context.Categories.Add(CreateCategory());
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetCategoryByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Category");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_Should_Throw_When_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () => await service.GetCategoryByIdAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Category not found");
        }
    }
}
