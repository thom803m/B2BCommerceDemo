using B2BCommerceDemo.Core.DTOs.Import;

namespace B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat
{
    public interface IRackbeatOrderSyncService
    {
        Task<ImportResult> SyncOrderAsync(int orderId, CancellationToken cancellationToken = default);
    }
}
