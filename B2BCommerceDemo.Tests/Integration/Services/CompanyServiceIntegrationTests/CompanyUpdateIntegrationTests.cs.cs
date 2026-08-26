using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CompanyServiceIntegrationTests
{
    public class CompanyUpdateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdateCompany_Should_Update_Name()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            await service.UpdateAsync(
                company.Id,
                new UpdateCompanyDto
                {
                    Name = "Apple Updated"
                });

            var result = await service.GetByIdAsync(company.Id);

            result.Name.Should().Be("Apple Updated");
        }

        [Fact]
        public async Task UpdateCompany_Should_Throw_When_Name_Already_Exists()
        {
            var service = GetService<CompanyService>();

            await service.CreateAsync(new CreateCompanyDto
            {
                Name = "Apple"
            });

            var company2 = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Samsung"
                });

            Func<Task> act = async () =>
                await service.UpdateAsync(
                    company2.Id,
                    new UpdateCompanyDto
                    {
                        Name = "Apple"
                    });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Company 'Apple' already exists");
        }
    }
}

