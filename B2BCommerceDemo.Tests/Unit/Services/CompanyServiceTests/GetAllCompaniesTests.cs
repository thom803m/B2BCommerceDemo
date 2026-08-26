using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class GetAllCompaniesTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task GetAllAsync_Should_Return_All_Active_Companies()
        {
            var context = CreateContext();

            context.Companies.AddRange(
                CreateCompany(1, "Company A", CompanyStatus.Active),
                CreateCompany(2, "Company B", CompanyStatus.Active));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_Should_Not_Return_Suspended_Companies()
        {
            var context = CreateContext();

            context.Companies.AddRange(
                CreateCompany(1, "Active Company", CompanyStatus.Active),
                CreateCompany(2, "Suspended Company", CompanyStatus.Suspended));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetAllAsync();

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Empty_List_When_No_Companies()
        {
            var context = CreateContext();

            var service = CreateService(context);

            var result = await service.GetAllAsync();

            result.Should().BeEmpty();
        }
    }
}

