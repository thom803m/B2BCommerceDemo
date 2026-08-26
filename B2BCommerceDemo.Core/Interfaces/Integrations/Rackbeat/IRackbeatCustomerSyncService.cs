namespace B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat
{
    public interface IRackbeatCustomerSyncService
    {
        Task<string> EnsureCustomerExistsAsync(int companyId, CancellationToken cancellationToken = default);
    }
}

