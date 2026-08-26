using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatPurchaseOrderSyncService : IRackbeatPurchaseOrderSyncService
    {
        private readonly IRackbeatClient _rackbeatClient;
        private readonly AppDbContext _context;
        private readonly IClock _clock;

        public RackbeatPurchaseOrderSyncService(
            IRackbeatClient rackbeatClient,
            AppDbContext context,
            IClock clock)
        {
            _rackbeatClient = rackbeatClient;
            _context = context;
            _clock = clock;
        }

        public async Task<ImportResult> SyncExpectedDeliveriesAsync(
            CancellationToken cancellationToken = default)
        {
            var result = new ImportResult();

            var deliveries = await _rackbeatClient.GetExpectedDeliveriesAsync(cancellationToken);

            if (!deliveries.Any())
            {
                result.Warnings.Add("No expected deliveries were returned from Rackbeat. Existing purchased quantities were not changed.");
                return result;
            }

            var oldestAcceptedDate = _clock.UtcNow.Date.AddMonths(-3);

            var grouped = deliveries
                .GroupBy(x => ProductImportNormalizer.NormalizeSku(x.Sku))
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        ExpectedDeliveryDate = g
                            .Where(x =>
                                x.ExpectedDeliveryDate.HasValue &&
                                x.ExpectedDeliveryDate.Value.Date >= oldestAcceptedDate)
                            .Select(x => x.ExpectedDeliveryDate)
                            .OrderBy(x => x)
                            .FirstOrDefault()
                    });

            var products = await _context.Products.ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.Sku))
                {
                    continue;
                }

                var sku = ProductImportNormalizer.NormalizeSku(product.Sku);

                var oldExpectedDeliveryDate = product.ExpectedDeliveryDate;

                if (grouped.TryGetValue(sku, out var delivery))
                {
                    product.ExpectedDeliveryDate =
                        product.PurchasedQuantity > 0
                            ? delivery.ExpectedDeliveryDate
                            : null;
                }
                else
                {
                    product.ExpectedDeliveryDate = null;
                }

                if (oldExpectedDeliveryDate != product.ExpectedDeliveryDate)
                {
                    result.Updated++;
                }

                if (product.PurchasedQuantity > 0 && product.ExpectedDeliveryDate == null)
                {
                    result.Warnings.Add($"SKU {product.Sku}: Has {product.PurchasedQuantity} purchased items but no expected delivery date in Rackbeat.");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
