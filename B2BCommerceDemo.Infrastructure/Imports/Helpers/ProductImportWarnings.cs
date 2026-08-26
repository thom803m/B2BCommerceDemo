using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Imports.Helpers
{
    public static class ProductImportWarnings
    {
        public static bool HasRequiredFieldWarnings(
            ImportResult result,
            string sku,
            string? ean,
            string? brand,
            string? category)
        {
            var missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(ean))
            {
                missingFields.Add("EAN");
            }

            if (string.IsNullOrWhiteSpace(brand))
            {
                missingFields.Add("Brand");
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                missingFields.Add("Category");
            }

            if (missingFields.Any())
            {
                result.Warnings.Add($"SKU {sku}: Missing required fields: {string.Join(", ", missingFields)}");

                return true;
            }

            return false;
        }

        public static async Task<bool> HasDuplicateEanWarning(
            AppDbContext context,
            ImportResult result,
            Dictionary<string, List<string>> seenEans,
            Product? existingProduct,
            string sku,
            string? ean)
        {
            if (string.IsNullOrWhiteSpace(ean) || ean.Trim() == "0") // 0 is considered invalid EAN
            {
                return false;
            }

            if (seenEans.ContainsKey(ean))
            {
                result.Warnings.Add($"SKU {sku}: Duplicate EAN {ean} found in import file");

                return true;
            }

            var duplicateExists = await context.Products
                .AnyAsync(p =>
                    p.Ean == ean &&
                    (existingProduct == null || p.Id != existingProduct.Id));

            if (duplicateExists)
            {
                result.Warnings.Add($"SKU {sku}: Duplicate EAN {ean} already exists");

                return true;
            }

            seenEans[ean] = new List<string> { sku };

            return false;
        }

        public static void AddScientificWarnings(
            ImportResult result,
            string sku,
            string? eanWarning)
        {
            if (!string.IsNullOrWhiteSpace(eanWarning))
            {
                result.Warnings.Add($"SKU {sku}: {eanWarning}");
            }
        }
    }
}
