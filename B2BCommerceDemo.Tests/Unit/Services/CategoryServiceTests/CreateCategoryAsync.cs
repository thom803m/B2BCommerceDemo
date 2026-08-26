using B2BCommerceDemo.Core.DTOs.Categories;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CategoryServiceTests
{
    public class CreateCategoryTests : CategoryServiceTestBase
    {
        [Fact]
        public async Task CreateCategoryAsync_Should_Create_Category()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var dto = new CreateCategoryDto
            {
                Name = " Electronics "
            };

            var result = await service.CreateCategoryAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Electronics");
        }

        [Fact]
        public async Task CreateCategoryAsync_Should_Throw_When_Name_Is_Empty()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var dto = new CreateCategoryDto
            {
                Name = "   "
            };

            Func<Task> act = async () => await service.CreateCategoryAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Category name cannot be empty");
        }

        [Fact]
        public async Task CreateCategoryAsync_Should_Call_Uniqueness_Validation()
        {
            var context = CreateContext();

            var validator = new Mock<IValidateUniqueness>(); ;

            validator.Setup(x =>
                    x.ValidateUniqueCategoryNameAsync("Moblies", null))
                .Returns(Task.CompletedTask);

            var service = CreateService(context, validator);

            var dto = new CreateCategoryDto
            {
                Name = "Moblies"
            };

            await service.CreateCategoryAsync(dto);

            validator.Verify(x =>
                x.ValidateUniqueCategoryNameAsync("Moblies", null),
                Times.Once);
        }
    }
}
