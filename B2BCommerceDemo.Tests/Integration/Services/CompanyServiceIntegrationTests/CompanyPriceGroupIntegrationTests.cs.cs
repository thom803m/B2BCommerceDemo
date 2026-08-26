using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CompanyServiceIntegrationTests
{
    public class CompanyPriceGroupIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task UpdatePriceGroup_Should_Update_PriceGroup()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            var group1 = await CreatePriceGroupAsync("Group1");
            var group2 = await CreatePriceGroupAsync("Group2");

            await service.UpdatePriceGroupAsync(
                company.Id,
                group1.Id);

            await service.UpdatePriceGroupAsync(
                company.Id,
                group2.Id);

            var fromDb = await Context.Companies.FindAsync(company.Id);

            fromDb!.PriceGroupId.Should().Be(group2.Id);
        }

        [Fact]
        public async Task UpdatePriceGroup_Should_Throw_When_PriceGroup_Not_Found()
        {
            var service = GetService<CompanyService>();

            var company = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            Func<Task> act = async () =>
                await service.UpdatePriceGroupAsync(
                    company.Id,
                    999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Price group not found");
        }
    }
}

