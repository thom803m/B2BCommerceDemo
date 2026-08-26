using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services.Helpers
{
    public class CompanyAccessValidator : ICompanyAccessValidator
    {
        private readonly AppDbContext _context;

        public CompanyAccessValidator(AppDbContext context)
        {
            _context = context;
        }

        public async Task ValidateCompanyActiveAsync(int companyId)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                throw new KeyNotFoundException("Company not found");
            }

            if (company.Status != CompanyStatus.Active)
            {
                throw new InvalidOperationException("Company is not approved");
            }
        }

        public async Task<Company> GetActiveCompanyAsync(int companyId)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .Include(c => c.PriceGroup)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                throw new KeyNotFoundException("Company not found");
            }

            if (company.Status != CompanyStatus.Active)
            {
                throw new InvalidOperationException("Company is not approved");
            }

            return company;
        }
    }
}

