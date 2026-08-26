using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.DTOs.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatOrderStatusSyncService : IRackbeatOrderStatusSyncService
    {
        private readonly AppDbContext _context;
        private readonly IRackbeatClient _rackbeatClient;

        public RackbeatOrderStatusSyncService(
            AppDbContext context,
            IRackbeatClient rackbeatClient)
        {
            _context = context;
            _rackbeatClient = rackbeatClient;
        }

        public async Task<ImportResult> SyncOrderStatusesAsync(CancellationToken cancellationToken = default)
        {
            var result = new ImportResult();

            var orders = await _context.Orders
                .Where(o => !string.IsNullOrWhiteSpace(o.RackbeatOrderNumber))
                .ToListAsync(cancellationToken);

            if (!orders.Any())
            {
                result.Warnings.Add("No Rackbeat-synced orders found.");
                return result;
            }

            foreach (var order in orders)
            {
                try
                {
                    var rackbeatOrder = await _rackbeatClient.GetOrderAsync(
                        order.RackbeatOrderNumber!,
                        cancellationToken);

                    if (rackbeatOrder == null)
                    {
                        result.Skipped++;
                        result.Warnings.Add($"Order {order.Id}: Rackbeat order {order.RackbeatOrderNumber} was not found.");

                        continue;
                    }

                    var newStatus = MapStatus(rackbeatOrder);

                    if (order.Status == newStatus)
                    {
                        result.Skipped++;
                        continue;
                    }

                    order.Status = newStatus;
                    result.Updated++;
                }
                catch (Exception ex)
                {
                    result.Skipped++;
                    result.Warnings.Add($"Order {order.Id}: Rackbeat status sync failed - {ex.Message}");
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }

        private static OrderStatus MapStatus(RackbeatOrderResponse rackbeatOrder)
        {
            if (rackbeatOrder.IsCancelled)
            {
                return OrderStatus.Cancelled;
            }

            if (rackbeatOrder.IsInvoiced)
            {
                return OrderStatus.Completed;
            }

            if (rackbeatOrder.IsShipped)
            {
                return OrderStatus.Shipped;
            }

            if (rackbeatOrder.IsReadyForShipping)
            {
                return OrderStatus.Processing;
            }

            if (rackbeatOrder.IsBooked)
            {
                return OrderStatus.Confirmed;
            }

            return OrderStatus.Pending;
        }
    }
}
