using B2BCommerceDemo.Core.DTOs.Import;

namespace B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat
{
    public interface IRackbeatPurchaseOrderSyncService
    {
        Task<ImportResult> SyncExpectedDeliveriesAsync(CancellationToken cancellationToken = default);
    }
}
