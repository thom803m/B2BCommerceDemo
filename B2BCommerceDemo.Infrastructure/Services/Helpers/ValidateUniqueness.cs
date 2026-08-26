using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services.Helpers
{
    public class ValidateUniqueness : IValidateUniqueness
    {
        private readonly AppDbContext _context;

        public ValidateUniqueness(AppDbContext context)
        {
            _context = context;
        }

        public async Task ValidateUniqueSkuAsync(string sku, int? excludeProductId = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new InvalidOperationException("SKU is required");
            }

            var normalizedSku = sku.Trim().ToUpperInvariant();

            var exists = await _context.Products
                .AnyAsync(p =>
                    p.Id != excludeProductId &&
                    p.Sku != null &&
                    p.Sku == normalizedSku);

            if (exists)
            {
                throw new InvalidOperationException($"Product with SKU '{sku}' already exists");
            }
        }

        public async Task ValidateUniqueEanAsync(string? ean, int? excludeProductId = null)
        {
            if (string.IsNullOrWhiteSpace(ean))
            {
                throw new InvalidOperationException("EAN is required");
            }

            var normalizedEan = ean.Trim();

            var exists = await _context.Products
                .AnyAsync(p =>
                    p.Id != excludeProductId &&
                    p.Ean != null &&
                    p.Ean == normalizedEan);

            if (exists)
            {
                throw new InvalidOperationException($"Product with EAN '{ean}' already exists");
            }
        }

        public async Task ValidateUniqueBrandNameAsync(string name, int? excludeBrandId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Brand name is required");
            }

            var normalized = name.Trim().ToUpperInvariant();

            var exists = await _context.Brands
                .AnyAsync(b =>
                    b.Id != excludeBrandId &&
                    b.Name != null &&
                    b.Name.ToUpper() == normalized);

            if (exists)
            {
                throw new InvalidOperationException($"Brand '{name}' already exists");
            }
        }
        public async Task ValidateUniqueCategoryNameAsync(string name, int? excludeCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Category name is required");
            }

            var normalized = name.Trim().ToUpperInvariant();

            var exists = await _context.Categories
                .AnyAsync(c =>
                    c.Id != excludeCategoryId &&
                    c.Name != null &&
                    c.Name.ToUpper() == normalized);

            if (exists)
            {
                throw new InvalidOperationException($"Category '{name}' already exists");
            }
        }

        public async Task ValidateUniqueCompanyNameAsync(string name, int? excludeCompanyId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Company name is required");
            }

            var normalized = name.Trim().ToUpperInvariant();

            var exists = await _context.Companies
                .AnyAsync(c =>
                    c.Id != excludeCompanyId &&
                    c.Name != null &&
                    c.Name.ToUpper() == normalized);

            if (exists)
            {
                throw new InvalidOperationException($"Company '{name}' already exists");
            }
        }
    }
}

