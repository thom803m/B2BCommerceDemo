using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.BrandServiceTests
{
    public class UpdateBrandTests : BrandServiceTestBase
    {
        [Fact]
        public async Task UpdateBrandAsync_Should_Update_Name()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateBrandDto
            {
                Name = " New Name "
            };

            var result = await service.UpdateBrandAsync(1, dto);

            result!.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task UpdateBrandAsync_Should_Throw_When_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var dto = new UpdateBrandDto
            {
                Name = "Test"
            };

            Func<Task> act =
                async () => await service.UpdateBrandAsync(99, dto);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Brand not found");
        }

        [Fact]
        public async Task UpdateBrandAsync_Should_Throw_When_Name_Empty()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateBrandDto
            {
                Name = "   "
            };

            Func<Task> act = async () =>
                await service.UpdateBrandAsync(1, dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Brand name cannot be empty");
        }

        [Fact]
        public async Task UpdateBrandAsync_Should_Call_Validation()
        {
            var context = CreateContext();

            context.Brands.Add(CreateBrand());

            await context.SaveChangesAsync();

            var validator = new Mock<IValidateUniqueness>();

            validator.Setup(x =>
                    x.ValidateUniqueBrandNameAsync("New Name", 1))
                .Returns(Task.CompletedTask);

            var service = CreateService(context, validator);

            var dto = new UpdateBrandDto
            {
                Name = " New Name "
            };

            await service.UpdateBrandAsync(1, dto);

            validator.Verify(x =>
                x.ValidateUniqueBrandNameAsync("New Name", 1),
                Times.Once);
        }
    }
}
