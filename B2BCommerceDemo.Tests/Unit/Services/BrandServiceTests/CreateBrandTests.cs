using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests
{
    public class CreateBrandTests : BrandServiceTestBase
    {
        [Fact]
        public async Task CreateBrandAsync_Should_Create_Brand()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var dto = new CreateBrandDto
            {
                Name = " Apple "
            };

            var result = await service.CreateBrandAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Apple");
        }

        [Fact]
        public async Task CreateBrandAsync_Should_Throw_When_Name_Is_Empty()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var dto = new CreateBrandDto
            {
                Name = "   "
            };

            Func<Task> act = async () => await service.CreateBrandAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Brand name cannot be empty");
        }

        [Fact]
        public async Task CreateBrandAsync_Should_Call_Uniqueness_Validation()
        {
            var context = CreateContext();

            var validator = new Mock<IValidateUniqueness>();

            validator.Setup(x =>
                    x.ValidateUniqueBrandNameAsync("Apple", null))
                .Returns(Task.CompletedTask);

            var service = CreateService(context, validator);

            var dto = new CreateBrandDto
            {
                Name = " Apple "
            };

            await service.CreateBrandAsync(dto);

            validator.Verify(x =>
                x.ValidateUniqueBrandNameAsync("Apple", null),
                Times.Once);
        }
    }
}
