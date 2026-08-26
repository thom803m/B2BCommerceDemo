using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class SuspendCompanyTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task SuspendAsync_Should_Suspend_Company()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(
                    1,
                    "Company A",
                    CompanyStatus.Active));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.SuspendAsync(1);

            var company = await context.Companies.FindAsync(1);

            company!.Status.Should()
                .Be(CompanyStatus.Suspended);
        }

        [Fact]
        public async Task SuspendAsync_Should_Throw_When_Company_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.SuspendAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Company with id 999 was not found.");
        }

        [Fact]
        public async Task SuspendAsync_Should_Throw_When_Company_Is_Already_Suspended()
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
                await service.SuspendAsync(1);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>();
        }
    }
}
