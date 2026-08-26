using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CompanyServiceIntegrationTests
{
    public class CompanyApproveIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ApproveCompany_Should_Set_Status_To_Active()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            var priceGroup = await CreatePriceGroupAsync();

            await service.ApproveCompanyAsync(
                company.Id,
                new ApproveCompanyDto
                {
                    PriceGroupId = priceGroup.Id,
                    RackbeatCustomerNumber = "900000580"
                });

            var fromDb = await Context.Companies.FindAsync(company.Id);

            fromDb!.Status.Should().Be(CompanyStatus.Active);
        }

        [Fact]
        public async Task ApproveCompany_Should_Set_PriceGroup()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            var priceGroup = await CreatePriceGroupAsync();

            await service.ApproveCompanyAsync(
                company.Id,
                new ApproveCompanyDto
                {
                    PriceGroupId = priceGroup.Id,
                    RackbeatCustomerNumber = "900000580"
                });

            var fromDb = await Context.Companies.FindAsync(company.Id);

            fromDb!.PriceGroupId.Should().Be(priceGroup.Id);
        }

        [Fact]
        public async Task ApproveCompany_Should_Throw_When_PriceGroup_Not_Found()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            Func<Task> act = async () =>
                await service.ApproveCompanyAsync(
                    company.Id,
                    new ApproveCompanyDto
                    {
                        PriceGroupId = 999,
                        RackbeatCustomerNumber = "900000580"
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Price group not found");
        }
    }
}

