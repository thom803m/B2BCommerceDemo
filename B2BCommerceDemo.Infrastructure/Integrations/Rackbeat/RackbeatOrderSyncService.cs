using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatOrderSyncService : IRackbeatOrderSyncService
    {
        private readonly AppDbContext _context;
        private readonly IRackbeatClient _rackbeatClient;
        private readonly IRackbeatCustomerSyncService _rackbeatCustomerSyncService;
        private readonly IClock _clock;

        public RackbeatOrderSyncService(
            AppDbContext context,
            IRackbeatClient rackbeatClient,
            IRackbeatCustomerSyncService rackbeatCustomerSyncService,
            IClock clock)
        {
            _context = context;
            _rackbeatClient = rackbeatClient;
            _rackbeatCustomerSyncService = rackbeatCustomerSyncService;
            _clock = clock;
        }

        public async Task<ImportResult> SyncOrderAsync(
            int orderId,
            CancellationToken cancellationToken = default)
        {
            var result = new ImportResult();

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order == null)
            {
                result.Skipped++;
                result.Warnings.Add($"Order {orderId} was not found.");
                return result;
            }

            if (!string.IsNullOrWhiteSpace(order.RackbeatOrderNumber))
            {
                result.Skipped++;
                result.Warnings.Add($"Order {order.Id} is already synced to Rackbeat as order {order.RackbeatOrderNumber}.");
                return result;
            }

            try
            {
                order.RackbeatSyncStatus = RackbeatSyncStatus.Pending;
                order.RackbeatSyncError = null;
                await _context.SaveChangesAsync(cancellationToken);

                var customerNumber = await _rackbeatCustomerSyncService
                    .EnsureCustomerExistsAsync(order.CompanyId, cancellationToken);

                var rackbeatOrderNumber = await _rackbeatClient.CreateOrderAsync(order, customerNumber, cancellationToken);

                order.RackbeatOrderNumber = rackbeatOrderNumber;
                order.RackbeatSyncStatus = RackbeatSyncStatus.Synced;
                order.RackbeatSyncError = null;
                order.RackbeatSyncedAt = _clock.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                result.Created++;
                return result;
            }
            catch (Exception ex)
            {
                order.RackbeatSyncStatus = RackbeatSyncStatus.Failed;
                order.RackbeatSyncError = ex.Message;

                await _context.SaveChangesAsync(cancellationToken);

                result.Skipped++;
                result.Warnings.Add($"Order {order.Id}: Rackbeat sync failed - {ex.Message}");

                return result;
            }
        }
    }
}
