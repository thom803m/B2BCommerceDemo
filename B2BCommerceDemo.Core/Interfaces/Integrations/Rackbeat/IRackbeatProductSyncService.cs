using B2BCommerceDemo.Core.DTOs.Import;

namespace B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat
{
    public interface IRackbeatProductSyncService
    {
        Task<ImportResult> SyncProductsAsync(CancellationToken cancellationToken = default);
    }
}

