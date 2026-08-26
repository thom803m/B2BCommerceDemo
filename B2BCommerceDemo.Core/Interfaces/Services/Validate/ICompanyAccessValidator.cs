using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Core.Interfaces.Services.Validate
{
    public interface ICompanyAccessValidator
    {
        Task ValidateCompanyActiveAsync(int companyId);
        Task<Company> GetActiveCompanyAsync(int companyId);
    }
}

