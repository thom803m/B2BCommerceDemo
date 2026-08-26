using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class PriceService : IPriceService
    {
        private readonly AppDbContext _context;
        private readonly ICompanyAccessValidator _companyAccessValidator;

        public PriceService(AppDbContext context, ICompanyAccessValidator companyAccessValidator)
        {
            _context = context;
            _companyAccessValidator = companyAccessValidator;
        }

        public async Task<decimal> GetPriceAsync(int productId, int companyId)
        {
            var company = await _companyAccessValidator
                .GetActiveCompanyAsync(companyId);

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new KeyNotFoundException("Product not found");

            var companyPrice = await _context.CompanyPrices
                .AsNoTracking()
                .FirstOrDefaultAsync(cp =>
                    cp.ProductId == productId &&
                    cp.CompanyId == companyId);

            if (companyPrice != null)
            {
                return companyPrice.Price;
            }

            var adjustment = company.PriceGroup?.PercentageAdjustment ?? 0m;
            var multiplier = 1 + (adjustment / 100m);

            return Math.Round(product.BasePrice * multiplier, 2);
        }

        public async Task<Dictionary<int, decimal>> GetPricesForProductsAsync(List<int> productIds, int companyId)
        {
            var company = await _companyAccessValidator
                .GetActiveCompanyAsync(companyId);


            var products = await _context.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var companyPrices = await _context.CompanyPrices
                .AsNoTracking()
                .Where(cp =>
                    productIds.Contains(cp.ProductId) &&
                    cp.CompanyId == companyId)
                .ToListAsync();

            var overrideDict = companyPrices
                .ToDictionary(x => x.ProductId, x => x.Price);

            var adjustment = company.PriceGroup?.PercentageAdjustment ?? 0m;
            var multiplier = 1 + (adjustment / 100m);

            var result = new Dictionary<int, decimal>();

            foreach (var p in products)
            {
                if (overrideDict.TryGetValue(p.Id, out var overridePrice))
                {
                    result[p.Id] = overridePrice;
                }
                else
                {
                    result[p.Id] = Math.Round(p.BasePrice * multiplier, 2);
                }
            }

            return result;
        }
    }
}
