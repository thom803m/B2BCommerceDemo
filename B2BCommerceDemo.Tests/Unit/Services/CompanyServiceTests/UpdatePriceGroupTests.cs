using B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Services.CompanyServiceTests
{
    public class UpdatePriceGroupTests : CompanyServiceTestBase
    {
        [Fact]
        public async Task UpdatePriceGroupAsync_Should_Update_PriceGroup()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A"));

            context.PriceGroups.Add(
                CreatePriceGroup(5));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.UpdatePriceGroupAsync(1, 5);

            var company = await context.Companies.FindAsync(1);

            company!.PriceGroupId.Should().Be(5);
        }

        [Fact]
        public async Task UpdatePriceGroupAsync_Should_Throw_When_Company_Not_Found()
        {
            var context = CreateContext();

            context.PriceGroups.Add(
                CreatePriceGroup(1));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdatePriceGroupAsync(999, 1);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Company with id 999 was not found.");
        }

        [Fact]
        public async Task UpdatePriceGroupAsync_Should_Throw_When_PriceGroup_Not_Found()
        {
            var context = CreateContext();

            context.Companies.Add(
                CreateCompany(1, "Company A"));

            await context.SaveChangesAsync();

            var service = CreateService(context);

            Func<Task> act = async () =>
                await service.UpdatePriceGroupAsync(1, 999);

            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Price group not found");
        }
    }
}
