using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatProductSyncService : IRackbeatProductSyncService
    {
        private readonly IRackbeatClient _rackbeatClient;
        private readonly IProductImportService _productImportService;

        public RackbeatProductSyncService(
            IRackbeatClient rackbeatClient,
            IProductImportService productImportService)
        {
            _rackbeatClient = rackbeatClient;
            _productImportService = productImportService;
        }

        public async Task<ImportResult> SyncProductsAsync(CancellationToken cancellationToken = default)
        {
            var products = await _rackbeatClient.GetProductsForImportAsync(cancellationToken);

            return await _productImportService.ImportRecordsAsync(products);
        }
    }
}

