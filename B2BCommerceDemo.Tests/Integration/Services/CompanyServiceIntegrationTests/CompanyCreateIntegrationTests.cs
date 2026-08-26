using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.CompanyServiceIntegrationTests
{
    public class CompanyCreateIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateCompany_Should_Create_Company()
        {
            var service = GetService<CompanyService>();

            var created = await service.CreateAsync(
                new CreateCompanyDto
                {
                    Name = "Apple"
                });

            var result = await service.GetByIdAsync(created.Id);

            result.Should().NotBeNull();
            result.Name.Should().Be("Apple");
        }

        [Fact]
        public async Task CreateCompany_Should_Throw_When_Name_Already_Exists()
        {
            var service = GetService<CompanyService>();

            await service.CreateAsync(new CreateCompanyDto
            {
                Name = "Apple"
            });

            Func<Task> act = async () =>
                await service.CreateAsync(new CreateCompanyDto
                {
                    Name = "apple"
                });

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Company 'apple' already exists");
        }
    }
}

