using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class GetPendingCompaniesTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task GetPendingCompaniesAsync_Should_Return_Pending_Companies()
        {
            var context = CreateContext();

            context.Companies.AddRange(
                CreateCompany(1, "Company A", CompanyStatus.Pending),
                CreateCompany(2, "Company B", CompanyStatus.Pending));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetPendingCompaniesAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetPendingCompaniesAsync_Should_Not_Return_Active_Companies()
        {
            var context = CreateContext();

            context.Companies.AddRange(
                CreateCompany(1, "Pending", CompanyStatus.Pending),
                CreateCompany(2, "Active", CompanyStatus.Active));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetPendingCompaniesAsync();

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1);
        }

        [Fact]
        public async Task GetPendingCompaniesAsync_Should_Return_Empty_List_When_No_Pending_Companies()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Active", CompanyStatus.Active));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetPendingCompaniesAsync();

            result.Should().BeEmpty();
        }
    }
}
