using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatCustomerSyncService : IRackbeatCustomerSyncService
    {
        private readonly AppDbContext _context;

        public RackbeatCustomerSyncService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> EnsureCustomerExistsAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);

            if (company == null)
            {
                throw new KeyNotFoundException($"Company {companyId} was not found.");
            }

            if (string.IsNullOrWhiteSpace(company.RackbeatCustomerNumber))
            {
                throw new InvalidOperationException($"Company {company.Id} has no Rackbeat customer number.");
            }

            return company.RackbeatCustomerNumber;
        }
    }
}
