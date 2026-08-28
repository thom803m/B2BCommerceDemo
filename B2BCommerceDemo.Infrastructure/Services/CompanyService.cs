using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IValidateUniqueness _validateUniqueness;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyService(AppDbContext context, IEventDispatcher eventDispatcher, IValidateUniqueness validateUniqueness, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
            _validateUniqueness = validateUniqueness;
            _userManager = userManager;
        }

        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var companies = await _context.Companies
                .AsNoTracking()
                .Include(c => c.PriceGroup)
                .Where(c => c.Status != CompanyStatus.Suspended)
                .ToListAsync();

            return companies
                .Select(CompanyMapper.MapToDto)
                .ToList();
        }

        public async Task<CompanyDto> GetByIdAsync(int id)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .Include(c => c.PriceGroup)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Status != CompanyStatus.Suspended);

            if (company == null)
            {
                throw new KeyNotFoundException($"Company with id {id} was not found.");
            }

            return CompanyMapper.MapToDto(company);
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            var name = dto.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Company name cannot be empty");
            }

            await _validateUniqueness.ValidateUniqueCompanyNameAsync(name);

            var company = new Company
            {
                Name = name
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CompanyMapper.MapToDto(company);
        }

        public async Task<CompanyDto> UpdateAsync(int id, UpdateCompanyDto dto)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Status != CompanyStatus.Suspended);

            if (company == null)
            {
                throw new KeyNotFoundException($"Company with id {id} was not found.");
            }

            var name = dto.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Company name cannot be empty");
            }

            await _validateUniqueness.ValidateUniqueCompanyNameAsync(name, id);

            company.Name = name;

            await _context.SaveChangesAsync();

            return CompanyMapper.MapToDto(company);
        }

        public async Task SuspendAsync(int id)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Status != CompanyStatus.Suspended);

            if (company == null)
            {
                throw new KeyNotFoundException($"Company with id {id} was not found.");
            }

            company.Status = CompanyStatus.Suspended;

            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanyDto>> GetPendingCompaniesAsync()
        {
            var companies = await _context.Companies
                .AsNoTracking()
                .Include(c => c.PriceGroup)
                .Where(c => c.Status == CompanyStatus.Pending)
                .ToListAsync();

            return companies
                .Select(CompanyMapper.MapToDto)
                .ToList();
        }

        public async Task<List<CompanyDto>> GetAdminCompaniesAsync()
        {
            var companies = await _context.Companies
                .AsNoTracking()
                .Include(c => c.PriceGroup)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return companies
                .Select(CompanyMapper.MapToDto)
                .ToList();
        }

        public async Task ApproveCompanyAsync(int companyId, ApproveCompanyDto dto)
        {
            var company = await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(
                    c => c.Id == companyId &&
                    c.Status != CompanyStatus.Suspended);

            if (company == null)
            {
                throw new KeyNotFoundException($"Company with id {companyId} was not found.");
            }

            var priceGroupExists = await _context.PriceGroups.AnyAsync(pg => pg.Id == dto.PriceGroupId);

            if (!priceGroupExists)
            {
                throw new KeyNotFoundException("Price group not found");
            }

            if (string.IsNullOrWhiteSpace(dto.RackbeatCustomerNumber))
            {
                throw new ArgumentException("Rackbeat customer number is required.");
            }

            company.PriceGroupId = dto.PriceGroupId;
            company.RackbeatCustomerNumber = dto.RackbeatCustomerNumber.Trim();
            company.Status = CompanyStatus.Active;

            await _context.SaveChangesAsync();

            var user = company.Users.FirstOrDefault();

            if (user != null)
            {
                await _eventDispatcher.PublishAsync(new CompanyApprovedEvent
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name!,
                    UserEmail = user.Email!
                });
            }
        }

        public async Task RejectCompanyAsync(int companyId)
        {
            var company = await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.CompanyPrices)
                .FirstOrDefaultAsync(c =>
                    c.Id == companyId &&
                    c.Status == CompanyStatus.Pending);

            if (company == null)
            {
                throw new KeyNotFoundException($"Pending company with id {companyId} was not found.");
            }

            var companyName = company.Name ?? "Unknown company";
            var userEmail = company.Users
                .Select(u => u.Email)
                .FirstOrDefault(email => !string.IsNullOrWhiteSpace(email));

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                await _eventDispatcher.PublishAsync(new CompanyRejectedEvent
                {
                    CompanyId = company.Id,
                    CompanyName = companyName,
                    UserEmail = userEmail
                });
            }

            foreach (var user in company.Users.ToList())
            {
                var deleteUserResult = await _userManager.DeleteAsync(user);

                if (!deleteUserResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", deleteUserResult.Errors.Select(e => e.Description)));
                }
            }

            if (company.CompanyPrices.Count > 0)
            {
                _context.CompanyPrices.RemoveRange(company.CompanyPrices);
            }

            _context.Companies.Remove(company);

            await _context.SaveChangesAsync();
        }

        public async Task ReactivateAsync(int id)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Status == CompanyStatus.Suspended);

            if (company == null)
            {
                throw new KeyNotFoundException($"Suspended company with id {id} was not found.");
            }

            company.Status = CompanyStatus.Active;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePriceGroupAsync(int companyId, int priceGroupId)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(
                    c => c.Id == companyId &&
                    c.Status != CompanyStatus.Suspended);

            if (company == null)
            {
                throw new KeyNotFoundException($"Company with id {companyId} was not found.");
            }

            var priceGroupExists = await _context.PriceGroups.AnyAsync(pg => pg.Id == priceGroupId);

            if (!priceGroupExists)
            {
                throw new KeyNotFoundException("Price group not found");
            }

            company.PriceGroupId = priceGroupId;

            await _context.SaveChangesAsync();
        }
    }
}
