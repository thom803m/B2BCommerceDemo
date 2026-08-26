using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CompanyServiceIntegrationTests
{
    public class CompanyRejectIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task RejectCompany_Should_Set_Status_To_Rejected()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            await service.RejectCompanyAsync(company.Id);

            var fromDb = await Context.Companies.FindAsync(company.Id);

            fromDb!.Status.Should().Be(CompanyStatus.Rejected);
        }
    }
}

