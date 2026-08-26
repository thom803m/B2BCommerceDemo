using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Imports;
using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Xml.Linq;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class ProductImportService : IProductImportService
    {
        private readonly AppDbContext _context;
        private readonly ProductImportCleanup _cleanup;
        private readonly ProductImportImageHandler _imageHandler;
        private readonly IClock _clock;

        public ProductImportService(
            AppDbContext context,
            ProductImportCleanup cleanup,
            ProductImportImageHandler imageHandler,
            IClock clock)
        {
            _context = context;
            _cleanup = cleanup;
            _imageHandler = imageHandler;
            _clock = clock;
        }

        public async Task<ImportResult> ImportCsvAsync(Stream filestream)
        {
            using var reader = new StreamReader(filestream);

            var config = new CsvConfiguration(new CultureInfo("da-DK"))
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ProductImportMap>();

            var records = csv.GetRecords<ProductImportDto>().ToList();

            return await ImportRecordsAsync(records);
        }

        public async Task<ImportResult> ImportXmlAsync(Stream filestream)
        {
            var doc = XDocument.Load(filestream);

            var records = doc.Descendants("Product")
                .Select(x => new ProductImportDto
                {
                    Sku = (string)x.Element("Sku") ?? "",
                    Name = (string)x.Element("Name") ?? "",
                    AvailableStock = (int?)x.Element("Available") ?? 0,
                    PurchasedQuantity = (int?)x.Element("Purchased") ?? 0,
                    Ean = (string)x.Element("Ean"),
                    BasePrice = (decimal?)x.Element("BasePrice") ?? 0,
                    Brand = (string)x.Element("Brand") ?? "",
                    Category = (string)x.Element("Category") ?? "",
                    ImageUrl = (string)x.Element("ImageUrl")
                })
                .ToList();

            return await ImportRecordsAsync(records);
        }

        public async Task<ImportResult> ImportRecordsAsync(List<ProductImportDto> records)
        {
            var result = new ImportResult();

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var seenEans = new Dictionary<string, List<string>>();

                var products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToListAsync();

                var existingProducts = products
                    .Where(p => !string.IsNullOrWhiteSpace(p.Sku))
                    .ToDictionary(
                        p => ProductImportNormalizer.NormalizeSku(p.Sku),
                        p => p
                    );

                var existingBrands = await _context.Brands
                    .Where(b => !string.IsNullOrWhiteSpace(b.Name))
                    .ToDictionaryAsync(
                        b => ProductImportNormalizer.NormalizeKey(b.Name),
                        b => b
                    );

                var existingCategories = await _context.Categories
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .ToDictionaryAsync(
                        c => ProductImportNormalizer.NormalizeKey(c.Name),
                        c => c
                    );

                var importedSkus = new HashSet<string>();

                var pendingImages = new List<(Product product, string? imageUrl)>();

                foreach (var r in records)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(r.Sku))
                        {
                            result.Skipped++;
                            continue;
                        }

                        await ImportRecordAsync(
                            r,
                            result,
                            existingProducts,
                            importedSkus,
                            seenEans,
                            existingBrands,
                            existingCategories,
                            pendingImages);
                    }
                    catch (Exception ex)
                    {
                        result.Skipped++;
                        result.Warnings.Add($"SKU {r.Sku}: Import failed - {ex.Message}");
                    }
                }

                if (!records.Any())
                {
                    throw new InvalidOperationException("Import aborted because the file contained no records.");
                }

                if (!importedSkus.Any())
                {
                    throw new InvalidOperationException("Import aborted because no valid products were imported.");
                }

                var activeProductCount = await _context.Products.CountAsync(p => p.IsActive);

                if (activeProductCount > 0)
                {
                    var importRatio = (double)
                        importedSkus.Count / activeProductCount;

                    if (importRatio < 0.5)
                    {
                        throw new InvalidOperationException(
                            $"Import aborted because only {importedSkus.Count} products were imported while {activeProductCount} active products currently exist.");
                    }
                }

                await _cleanup.DeactivateMissingProductsAsync(importedSkus);
                await _cleanup.CleanupOldProductsAsync();

                await _context.SaveChangesAsync();

                foreach (var (product, imageUrl) in pendingImages)
                {
                    await _imageHandler.HandleImagesAsync(product.Id, imageUrl);
                }

                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ImportRecordAsync(
            ProductImportDto r,
            ImportResult result,
            Dictionary<string, Product> existingProducts,
            HashSet<string> importedSkus,
            Dictionary<string, List<string>> seenEans,
            Dictionary<string, Brand> existingBrands,
            Dictionary<string, Category> existingCategories,
            List<(Product product, string? imageUrl)> pendingImages)
        {
            ApplyPurchaseRules(r);

            var sku = ProductImportNormalizer.NormalizeSku(r.Sku);

            if (ProductImportNormalizer.IsDamagedBox(sku))
            {
                result.Skipped++;
                return;
            }

            var (ean, eanWarning) = ProductImportNormalizer.NormalizeScientific(r.Ean);

            if (string.IsNullOrWhiteSpace(ean) || ean.Trim() == "0") // 0 is considered invalid EAN
            {
                ean = null;
            }

            existingProducts.TryGetValue(sku, out var existing);

            var hasStock = r.AvailableStock > 0 || r.PurchasedQuantity > 0;
            var hasPrice = r.BasePrice > 0;

            if (!hasStock || !hasPrice)
            {
                result.Skipped++;
                return;
            }

            if (ProductImportWarnings.HasRequiredFieldWarnings(result, sku, ean, r.Brand, r.Category))
            {
                result.Skipped++;
                return;
            }

            if (await ProductImportWarnings.HasDuplicateEanWarning(_context, result, seenEans, existing, sku, ean))
            {
                result.Skipped++;
                return;
            }

            importedSkus.Add(sku);

            var brand = await GetOrCreateBrandAsync(r.Brand, existingBrands);
            var category = await GetOrCreateCategoryAsync(r.Category, existingCategories);

            if (existing == null)
            {
                var product = await CreateProductAsync(r, sku, ean, brand, category);

                existingProducts[sku] = product;

                pendingImages.Add((product, r.ImageUrl));

                result.Created++;
            }
            else
            {
                UpdateProduct(existing, r, ean, brand, category);

                pendingImages.Add((existing, r.ImageUrl));

                result.Updated++;
            }

            ProductImportWarnings.AddScientificWarnings(result, sku, eanWarning);
        }

        private static void ApplyPurchaseRules(ProductImportDto r)
        {
            r.AvailableStock = Math.Max(0, r.AvailableStock);
            r.PurchasedQuantity = Math.Max(0, r.PurchasedQuantity);

            if (r.PurchasedQuantity == 0)
            {
                r.ExpectedDeliveryDate = null;
            }
        }

        private async Task<Product> CreateProductAsync(
            ProductImportDto r,
            string sku,
            string? ean,
            Brand? brand,
            Category? category)
        {
            var product = new Product
            {
                Sku = sku,
                Name = r.Name?.Trim() ?? "",
                BasePrice = r.BasePrice,
                Ean = ean,
                AvailableStock = Math.Max(0, r.AvailableStock),
                PurchasedQuantity = Math.Max(0, r.PurchasedQuantity),

                ExpectedDeliveryDate = r.PurchasedQuantity > 0
                    ? r.ExpectedDeliveryDate
                    : null,

                Brand = brand,
                Category = category,
                IsActive = true,
                LastSynced = _clock.UtcNow,

                // Content metadata
                ContentSource = "Rackbeat",
                ContentLocked = false
            };

            await _context.Products.AddAsync(product);

            return product;
        }

        // Rackbeat sync must only update commercial product data.
        // Enriched/manual content fields are intentionally preserved.
        private void UpdateProduct(
            Product product,
            ProductImportDto r,
            string? ean,
            Brand? brand,
            Category? category)
        {
            product.Name = r.Name?.Trim() ?? "";
            product.BasePrice = r.BasePrice;
            product.Ean = ean;
            product.AvailableStock = Math.Max(0, r.AvailableStock);
            product.PurchasedQuantity = Math.Max(0, r.PurchasedQuantity);

            if (product.PurchasedQuantity == 0)
            {
                product.ExpectedDeliveryDate = null;
            }
            else
            {
                product.ExpectedDeliveryDate = r.ExpectedDeliveryDate;
            }

            product.Brand = brand;
            product.Category = category;
            product.IsActive = true;
            product.LastSynced = _clock.UtcNow;
        }

        private async Task<Brand?> GetOrCreateBrandAsync(
            string? brandName,
            Dictionary<string, Brand> existingBrands)
        {
            if (string.IsNullOrWhiteSpace(brandName))
            {
                return null;
            }

            var normalized = ProductImportNormalizer.NormalizeKey(brandName);

            if (existingBrands.TryGetValue(normalized, out var brand))
            {
                return brand;
            }

            brand = new Brand { Name = brandName.Trim() };

            existingBrands[normalized] = brand;

            await _context.Brands.AddAsync(brand);

            return brand;
        }

        private async Task<Category?> GetOrCreateCategoryAsync(
            string? categoryName,
            Dictionary<string, Category> existingCategories)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return null;
            }

            var normalized = ProductImportNormalizer.NormalizeKey(categoryName);

            if (existingCategories.TryGetValue(normalized, out var category))
            {
                return category;
            }

            category = new Category { Name = categoryName.Trim() };

            existingCategories[normalized] = category;

            await _context.Categories.AddAsync(category);

            return category;
        }
    }
}
