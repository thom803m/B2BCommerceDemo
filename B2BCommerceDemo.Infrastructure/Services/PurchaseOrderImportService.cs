using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Imports;
using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class PurchaseOrderImportService : IPurchaseOrderImportService
    {
        private readonly AppDbContext _context;
        private readonly IClock _clock;

        public PurchaseOrderImportService(AppDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task<ImportResult> ImportCsvAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);

            var config = new CsvConfiguration(new CultureInfo("da-DK"))
            {
                HasHeaderRecord = true,
                Delimiter = ";",
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<PurchaseOrderImportMap>();

            var records = csv.GetRecords<PurchaseOrderImportDto>().ToList();

            return await ProcessRecords(records);
        }

        private async Task<ImportResult> ProcessRecords(List<PurchaseOrderImportDto> records)
        {
            var result = new ImportResult();

            var oldestAcceptedDate = _clock.UtcNow.Date.AddMonths(-3);

            var skuDeliveryMap = records
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.Sku) &&
                    x.ExpectedDeliveryDate.HasValue &&
                    x.ExpectedDeliveryDate.Value.Date >= oldestAcceptedDate &&
                    IsUnreceivedLine(x))
                .GroupBy(x => ProductImportNormalizer.NormalizeSku(x.Sku))
                .ToDictionary(
                    g => g.Key,
                    g => g.Min(x => x.ExpectedDeliveryDate!.Value.Date)
                );

            var products = await _context.Products.ToListAsync();

            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.Sku))
                    continue;

                var sku = ProductImportNormalizer.NormalizeSku(product.Sku);
                var oldExpectedDeliveryDate = product.ExpectedDeliveryDate;

                if (product.PurchasedQuantity == 0)
                {
                    product.ExpectedDeliveryDate = null;
                }
                else if (skuDeliveryMap.TryGetValue(sku, out var deliveryDate))
                {
                    product.ExpectedDeliveryDate = deliveryDate;
                }
                else
                {
                    product.ExpectedDeliveryDate = null;

                    result.Warnings.Add(
                        $"SKU {product.Sku}: Has {product.PurchasedQuantity} purchased items but no expected delivery date in purchase order CSV.");
                }

                if (oldExpectedDeliveryDate != product.ExpectedDeliveryDate)
                {
                    product.LastSynced = _clock.UtcNow;
                    result.Updated++;
                }
            }

            await _context.SaveChangesAsync();

            return result;
        }

        private static bool IsUnreceivedLine(PurchaseOrderImportDto record)
        {
            if (!record.Quantity.HasValue)
            {
                return true;
            }

            var receivedQuantity =
                record.ReceivedQuantity ??
                record.InvoicedQuantity ??
                0;

            return record.Quantity.Value - receivedQuantity > 0;
        }
    }
}
