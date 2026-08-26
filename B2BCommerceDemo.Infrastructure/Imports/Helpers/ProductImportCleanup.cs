using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Imports.Helpers
{
    public class ProductImportCleanup
    {
        private readonly AppDbContext _context;
        private readonly IClock _clock;

        public ProductImportCleanup(
            AppDbContext context,
            IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task DeactivateMissingProductsAsync(HashSet<string> importedSkus)
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();

            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.Sku))
                    continue;

                var normalizedSku =
                    ProductImportNormalizer.NormalizeSku(product.Sku);

                if (!importedSkus.Contains(normalizedSku))
                {
                    product.IsActive = false;
                }
            }
        }

        public async Task CleanupOldProductsAsync()
        {
            var cutoffDate = _clock.UtcNow.AddDays(-90);

            await _context.Products
                .Where(p => p.IsActive && p.LastSynced < cutoffDate)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsActive,false));
        }
    }
}

