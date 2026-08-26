using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class GetCompanyByIdTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task GetByIdAsync_Should_Return_Company()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Company A");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_When_Company_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.GetByIdAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Company with id 999 was not found.");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_When_Company_Is_Suspended()
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
                await service.GetByIdAsync(1);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Company with id 1 was not found.");
        }
    }
}

