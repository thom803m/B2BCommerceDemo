using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class ApproveCompanyTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task ApproveCompanyAsync_Should_Approve_Company()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A", CompanyStatus.Pending));

            context.PriceGroups.Add(
                CreatePriceGroup(1));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.ApproveCompanyAsync(
                1,
                new ApproveCompanyDto
                {
                    PriceGroupId = 1,
                    RackbeatCustomerNumber = "900000580"
                });

            var company = await context.Companies.FindAsync(1);

            company!.Status.Should().Be(CompanyStatus.Active);
        }

        [Fact]
        public async Task ApproveCompanyAsync_Should_Set_PriceGroup()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A", CompanyStatus.Pending));

            context.PriceGroups.Add(
                CreatePriceGroup(5));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.ApproveCompanyAsync(
                1,
                new ApproveCompanyDto
                {
                    PriceGroupId = 5,
                    RackbeatCustomerNumber = "900000580"
                });

            var company = await context.Companies.FindAsync(1);

            company!.PriceGroupId.Should().Be(5);
        }

        [Fact]
        public async Task ApproveCompanyAsync_Should_Throw_When_Company_Not_Found()
        {
            var context = CreateContext();

            context.PriceGroups.Add(
                CreatePriceGroup(1));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.ApproveCompanyAsync(
                    999, 
                    new ApproveCompanyDto 
                    { 
                        PriceGroupId = 1, 
                        RackbeatCustomerNumber = "900000580" 
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Company with id 999 was not found.");
        }

        [Fact]
        public async Task ApproveCompanyAsync_Should_Throw_When_PriceGroup_Not_Found()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A", CompanyStatus.Pending));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.ApproveCompanyAsync(
                    1, 
                    new ApproveCompanyDto 
                    { 
                        PriceGroupId = 999, 
                        RackbeatCustomerNumber = "900000580" 
                    });

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Price group not found");
        }

        [Fact]
        public async Task ApproveCompanyAsync_Should_Publish_CompanyApprovedEvent()
        {
            var context = CreateContext();

            var company = CreateCompany(1, "Company A", CompanyStatus.Pending);

            company.Users = new List<ApplicationUser>
            {
                new()
                {
                    Email = "test@test.dk"
                }
            };

            context.Companies.Add(company);

            context.PriceGroups.Add(
                CreatePriceGroup(1));

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.ApproveCompanyAsync(
                1,
                new ApproveCompanyDto
                {
                    PriceGroupId = 1,
                    RackbeatCustomerNumber = "900000580"
                });

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<CompanyApprovedEvent>(e =>
                        e.CompanyId == 1 &&
                        e.CompanyName == "Company A" &&
                        e.UserEmail == "test@test.dk")),
                Times.Once);
        }

        [Fact]
        public async Task ApproveCompanyAsync_Should_Set_RackbeatCustomerNumber()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A", CompanyStatus.Pending));

            context.PriceGroups.Add(
                CreatePriceGroup(1));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.ApproveCompanyAsync(
                1,
                new ApproveCompanyDto
                {
                    PriceGroupId = 1,
                    RackbeatCustomerNumber = "900000580"
                });

            var company = await context.Companies.FindAsync(1);

            company!.RackbeatCustomerNumber.Should().Be("900000580");
        }
    }
}
