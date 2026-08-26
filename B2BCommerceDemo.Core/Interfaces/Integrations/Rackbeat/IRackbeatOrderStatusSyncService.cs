using B2BCommerceDemo.Core.DTOs.Import;

namespace B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat
{
    public interface IRackbeatOrderStatusSyncService
    {
        Task<ImportResult> SyncOrderStatusesAsync(CancellationToken cancellationToken = default);
    }
}
