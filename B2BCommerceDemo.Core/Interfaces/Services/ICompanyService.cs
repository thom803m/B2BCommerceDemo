using B2BCommerceDemo.Core.DTOs.Companies;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllAsync();
        Task<CompanyDto> GetByIdAsync(int id);
        Task<CompanyDto> CreateAsync(CreateCompanyDto dto);
        Task<CompanyDto> UpdateAsync(int id, UpdateCompanyDto dto);
        Task SuspendAsync(int id);

        Task<List<CompanyDto>> GetPendingCompaniesAsync();
        Task<List<CompanyDto>> GetAdminCompaniesAsync();
        Task ApproveCompanyAsync(int companyId, ApproveCompanyDto dto);
        Task RejectCompanyAsync(int companyId);
        Task ReactivateAsync(int id);
        Task UpdatePriceGroupAsync(int companyId, int priceGroupId);
    }
}

