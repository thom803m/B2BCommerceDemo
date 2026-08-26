using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class UpdateCompanyTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task UpdateAsync_Should_Update_Name()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Old Name"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.UpdateAsync(
                1,
                new UpdateCompanyDto
                {
                    Name = "New Name"
                });

            result.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task UpdateAsync_Should_Trim_Name()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Old Name"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.UpdateAsync(
                1,
                new UpdateCompanyDto
                {
                    Name = "  New Name  "
                });

            result.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Company_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateAsync(
                    999,
                    new UpdateCompanyDto
                    {
                        Name = "New Name"
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Company with id 999 was not found.");
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Company_Is_Suspended()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(
                    1,
                    "Company A",
                    CompanyStatus.Suspended));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdateAsync(
                    1,
                    new UpdateCompanyDto
                    {
                        Name = "New Name"
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_Should_Validate_Unique_Name()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A"));

            await context.SaveChangesAsync();

            var validator = CreateUniquenessValidator();

            var service = CreateService(
                context,
                validate: validator);

            await service.UpdateAsync(
                1,
                new UpdateCompanyDto
                {
                    Name = "New Name"
                });

            validator.Verify(
                x => x.ValidateUniqueCompanyNameAsync(
                    "New Name",
                    1),
                Times.Once);
        }
    }
}
