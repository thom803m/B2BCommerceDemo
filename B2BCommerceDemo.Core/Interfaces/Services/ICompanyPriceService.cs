using B2BCommerceDemo.Core.DTOs.Companies;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface ICompanyPriceService
    {
        Task<List<CompanyPriceDto>> GetAllAsync();

        Task<CompanyPriceDto> CreateAsync(CreateCompanyPriceDto dto);

        Task<CompanyPriceDto> UpdateAsync(int id, UpdateCompanyPriceDto dto);

        Task DeleteAsync(int id);
    }
}

