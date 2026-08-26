using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class CreateCompanyTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task CreateAsync_Should_Create_Company()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var result = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Company A"
                });

            result.Name.Should().Be("Company A");

            context.Companies.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateAsync_Should_Trim_Name()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var result = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "  Company A  "
                });

            result.Name.Should().Be("Company A");
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Name_Is_Empty()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.CreateAsync(
                    new CreateCompanyDto
                    {
                        Name = "   "
                    });

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Company name cannot be empty");
        }

        [Fact]
        public async Task CreateAsync_Should_Validate_Unique_Name()
        {
            var context = CreateContext();

            var validator = CreateUniquenessValidator();

            var service = CreateService(
                context,
                validate: validator);

            await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Company A"
                });

            validator.Verify(
                x => x.ValidateUniqueCompanyNameAsync(
                    "Company A",
                    null),
                Times.Once);
        }
    }
}
