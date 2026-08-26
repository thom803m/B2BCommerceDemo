using B2BCommerceDemo.Core.DTOs.Categories;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests
{
    public class UpdateCategoryTests : CategoryServiceTestBase
    {
        [Fact]
        public async Task UpdateCategoryAsync_Should_Update_Category()
        {
            var context = CreateContext();

            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateCategoryDto
            {
                Name = " New Name "
            };

            var result = await service.UpdateCategoryAsync(1, dto);

            result!.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_Throw_When_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var dto = new UpdateCategoryDto
            {
                Name = "Test"
            };

            Func<Task> act = async () =>
                await service.UpdateCategoryAsync(999, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Category not found");
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_Throw_When_Name_Empty()
        {
            var context = CreateContext();

            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateCategoryDto
            {
                Name = "   "
            };

            Func<Task> act = async () =>
                await service.UpdateCategoryAsync(1, dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Category name cannot be empty");
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_Call_Validation()
        {
            var context = CreateContext();

            context.Categories.Add(CreateCategory());

            await context.SaveChangesAsync();

            var validator = new Mock<IValidateUniqueness>();

            validator.Setup(x =>
                    x.ValidateUniqueCategoryNameAsync("New Name", 1))
                .Returns(Task.CompletedTask);

            var service = CreateService(context, validator);

            var dto = new UpdateCategoryDto
            {
                Name = "New Name"
            };

            await service.UpdateCategoryAsync(1, dto);

            validator.Verify(x =>
                x.ValidateUniqueCategoryNameAsync("New Name", 1),
                Times.Once);
        }
    }
}
