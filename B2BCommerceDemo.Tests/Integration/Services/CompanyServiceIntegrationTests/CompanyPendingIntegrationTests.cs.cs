using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CompanyServiceIntegrationTests
{
    public class CompanyPendingIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetPendingCompanies_Should_Return_Only_Pending()
        {
            Context.Companies.AddRange(
                new Company
                {
                    Name = "Pending",
                    Status = CompanyStatus.Pending
                },
                new Company
                {
                    Name = "Active",
                    Status = CompanyStatus.Active
                });

            await Context.SaveChangesAsync();

            var service = GetService<CompanyService>();

            var result = await service.GetPendingCompaniesAsync();

            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Pending");
        }
    }
}

