using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;
using Moq;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class RejectCompanyTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task RejectCompanyAsync_Should_Reject_Company()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A", CompanyStatus.Pending));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.RejectCompanyAsync(1);

            var company = await context.Companies.FindAsync(1);

            company!.Status.Should().Be(CompanyStatus.Rejected);
        }

        [Fact]
        public async Task RejectCompanyAsync_Should_Throw_When_Company_Not_Found()
        {
            var context = CreateContext();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.RejectCompanyAsync(999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Company with id 999 was not found.");
        }

        [Fact]
        public async Task RejectCompanyAsync_Should_Publish_CompanyRejectedEvent()
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

            await context.SaveChangesAsync();

            var dispatcher = new Mock<IEventDispatcher>();

            var service = CreateService(
                context,
                eventDispatcher: dispatcher);

            await service.RejectCompanyAsync(1);

            dispatcher.Verify(
                x => x.PublishAsync(
                    It.Is<CompanyRejectedEvent>(e =>
                        e.CompanyId == 1 &&
                        e.CompanyName == "Company A" &&
                        e.UserEmail == "test@test.dk")),
                Times.Once);
        }
    }
}
