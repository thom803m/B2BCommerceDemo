using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.DTOs.PriceGroups;
using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Infrastructure.Mappers
{
    public static class CompanyMapper
    {
        public static CompanyDto MapToDto(Company company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name ?? string.Empty,
                Status = company.Status.ToString(),
                RackbeatCustomerNumber = company.RackbeatCustomerNumber,
                PriceGroup = company.PriceGroup is null
                    ? null
                    : new PriceGroupDto
                    {
                        Id = company.PriceGroup.Id,
                        Name = company.PriceGroup.Name ?? string.Empty,
                        PercentageAdjustment = company.PriceGroup.PercentageAdjustment
                    }
            };
        }
    }
}
