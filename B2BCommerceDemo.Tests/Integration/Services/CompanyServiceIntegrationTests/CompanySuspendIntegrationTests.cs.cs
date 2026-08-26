using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CompanyServiceIntegrationTests
{
    public class CompanySuspendIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task SuspendCompany_Should_Set_Status_To_Suspended()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            await service.SuspendAsync(company.Id);

            Func<Task> act = async () =>
                await service.GetByIdAsync(company.Id);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>();
        }
    }
}

