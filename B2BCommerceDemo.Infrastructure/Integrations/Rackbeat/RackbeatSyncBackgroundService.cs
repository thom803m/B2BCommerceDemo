using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RackbeatSyncBackgroundService> _logger;

        private static readonly TimeSpan SyncInterval = TimeSpan.FromHours(1);

        public RackbeatSyncBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<RackbeatSyncBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Rackbeat background sync service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunSyncAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Rackbeat background sync failed.");
                }

                await Task.Delay(SyncInterval, stoppingToken);
            }
        }

        private async Task RunSyncAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var productSyncService = scope.ServiceProvider.GetRequiredService<IRackbeatProductSyncService>();

            var purchaseOrderSyncService = scope.ServiceProvider.GetRequiredService<IRackbeatPurchaseOrderSyncService>();

            var orderStatusSyncService = scope.ServiceProvider.GetRequiredService<IRackbeatOrderStatusSyncService>();

            await RunSyncStepAsync(
                "product",
                () => productSyncService.SyncProductsAsync(cancellationToken));

            await RunSyncStepAsync(
                "expected delivery",
                () => purchaseOrderSyncService.SyncExpectedDeliveriesAsync(cancellationToken));

            await RunSyncStepAsync(
                "order status",
                () => orderStatusSyncService.SyncOrderStatusesAsync(cancellationToken));
        }

        private async Task RunSyncStepAsync(
            string name,
            Func<Task<ImportResult>> syncAction)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Rackbeat {Name} sync started.", name);

                var result = await syncAction();

                stopwatch.Stop();

                _logger.LogInformation(
                    "Rackbeat {Name} sync completed in {ElapsedMs} ms. Created: {Created}, Updated: {Updated}, Skipped: {Skipped}, Warnings: {Warnings}",
                    name,
                    stopwatch.ElapsedMilliseconds,
                    result.Created,
                    result.Updated,
                    result.Skipped,
                    result.Warnings.Count);

                foreach (var warning in result.Warnings)
                {
                    _logger.LogWarning(
                        "Rackbeat {Name} sync warning: {Warning}",
                        name,
                        warning);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Rackbeat {Name} sync failed after {ElapsedMs} ms.",
                    name,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
