using B2BCommerceDemo.Core.Exports;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class ProductExportService : IProductExportService
    {
        private readonly AppDbContext _context;
        private readonly List<ExportFieldDefinition> _fields;
        private readonly IPriceService _priceService;
        private readonly IClock _clock;

        public ProductExportService(AppDbContext context, IClock clock, IPriceService priceService)
        {
            _context = context;
            _clock = clock;
            _priceService = priceService;

            _fields =
            [
                new()
                {
                    Key = "sku",
                    Header = "SKU",
                    Selector = p => p.Sku
                },
                new()
                {
                    Key = "name",
                    Header = "Name",
                    Selector = p =>
                        !string.IsNullOrWhiteSpace(p.IcecatName)
                            ? p.IcecatName
                            : p.Name
                },
                new()
                {
                    Key = "stock",
                    Header = "Stock",
                    Selector = p => FormatStock(p.AvailableStock)
                },
                new()
                {
                    Key = "purchase",
                    Header = "Incoming",
                    Selector = p => FormatIncomingStock(p.PurchasedQuantity)
                },
                new()
                {
                    Key = "ean",
                    Header = "EAN",
                    Selector = p => string.IsNullOrWhiteSpace(p.Ean)
                        ? ""
                        : $"=\"{p.Ean}\""
                },
                new()
                {
                    Key = "price",
                    Header = "Price EUR",
                    Selector = p => FormatPrice(p.BasePrice)
                },
                new()
                {
                    Key = "brand",
                    Header = "Manufacturer",
                    Selector = p => p.Brand?.Name
                },
                new()
                {
                    Key = "category",
                    Header = "Product Category",
                    Selector = p => p.Category?.Name
                },
                new()
                {
                    Key = "delivery",
                    Header = "ETA",
                    Selector = p => FormatDeliveryDate(p.PurchasedQuantity, p.ExpectedDeliveryDate)
                }
            ];
        }

        public List<ExportFieldDefinition> GetAvailableFields()
        {
            return _fields;
        }

        public Task<byte[]> ExportProductsToCsvAsync(List<string>? selectedFields = null, int? companyId = null)
        {
            return ExportInternalAsync(selectedFields, null, companyId);
        }

        public Task<byte[]> ExportProductsWithMarkupToCsvAsync(
            List<string> selectedFields,
            decimal percentage)
        {
            return ExportInternalAsync(selectedFields, percentage);
        }

        private async Task<byte[]> ExportInternalAsync(List<string>? selectedFields, decimal? percentage, int? companyId = null)
        {
            var includeAvailable = selectedFields?.Contains("stock") ?? false;
            var includePurchased = selectedFields?.Contains("purchase") ?? false;

            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Include(p => p.Brand)
                .Include(p => p.Category);

            if (includeAvailable && !includePurchased)
            {
                query = query.Where(p => p.AvailableStock > 0);
            }
            else if (includePurchased && !includeAvailable)
            {
                query = query.Where(p => p.PurchasedQuantity > 0);
            }

            query = query.OrderBy(p => p.IcecatName ?? p.Name);

            var products = await query.ToListAsync();

            Dictionary<int, decimal> companyPrices = new();

            if (!products.Any())
            {
                throw new InvalidOperationException("No active products found.");
            }

            if (companyId.HasValue)
            {
                companyPrices = await _priceService
                    .GetPricesForProductsAsync(
                        products.Select(p => p.Id).ToList(),
                        companyId.Value);
            }

            var fieldLookup = _fields.ToDictionary(f => f.Key);

            var invalidFields = selectedFields?
                .Where(f => !fieldLookup.ContainsKey(f))
                .ToList();

            if (invalidFields is { Count: > 0 })
            {
                throw new ArgumentException($"Invalid export fields: {string.Join(", ", invalidFields)}");
            }

            var fields = selectedFields is { Count: > 0 }
                ? selectedFields
                    .Select(key => fieldLookup[key])
                    .ToList()
                : _fields;

            using var memoryStream = new MemoryStream();

            using var writer = new StreamWriter(
                memoryStream,
                new UTF8Encoding(true));

            using var csv = new CsvWriter(writer, new CsvConfiguration(new CultureInfo("da-DK"))
            {
                Delimiter = ";"
            });

            foreach (var field in fields)
            {
                csv.WriteField(field.Header);
            }

            await csv.NextRecordAsync();

            foreach (var product in products)
            {
                foreach (var field in fields)
                {
                    string? value;

                    if (field.Key == "price")
                    {
                        decimal price = product.BasePrice;

                        if (companyId.HasValue &&
                            companyPrices.TryGetValue(product.Id, out var companyPrice))
                        {
                            price = companyPrice;
                        }

                        if (percentage.HasValue)
                        {
                            price *= 1 + (percentage.Value / 100m);
                        }

                        value = Math.Round(price, 0, MidpointRounding.AwayFromZero)
                            .ToString("0", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        value = field.Selector(product);
                    }

                    csv.WriteField(value);
                }

                await csv.NextRecordAsync();
            }

            await writer.FlushAsync();

            return memoryStream.ToArray();
        }

        private static string FormatStock(int stock)
        {
            if (stock < 0)
            {
                return "0";
            }

            if (stock > 100)
            {
                return "100+";
            }

            return stock.ToString();
        }

        private static string FormatIncomingStock(int quantity)
        {
            if (quantity <= 0)
            {
                return "0";
            }

            if (quantity > 100)
            {
                return "100+";
            }

            return quantity.ToString();
        }

        private static string FormatPrice(decimal price)
        {
            return Math.Round(price, 0, MidpointRounding.AwayFromZero).ToString("0", CultureInfo.InvariantCulture);
        }

        private string FormatDeliveryDate(int purchasedQuantity, DateTime? deliveryDate)
        {
            if (purchasedQuantity <= 0)
            {
                return "";
            }

            if (!deliveryDate.HasValue)
            {
                return "To be confirmed";
            }

            var today = _clock.UtcNow.Date;

            if (deliveryDate.Value.Date < today)
            {
                return "To be confirmed";
            }

            return deliveryDate.Value.ToString("dd-MM-yyyy");
        }
    }
}
