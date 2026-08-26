using B2BCommerceDemo.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace B2BCommerceDemo.Infrastructure.Integrations.Icecat
{
    public class IcecatSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IcecatSyncBackgroundService> _logger;

        private static readonly TimeSpan SyncInterval = TimeSpan.FromHours(24);

        public IcecatSyncBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<IcecatSyncBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Icecat background sync service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunSyncAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Icecat background sync failed.");
                }

                await Task.Delay(SyncInterval, stoppingToken);
            }
        }

        private async Task RunSyncAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var enrichmentService =
                scope.ServiceProvider.GetRequiredService<IProductContentEnrichmentService>();

            _logger.LogInformation("Icecat enrichment sync started.");

            var result = await enrichmentService
                .EnrichMissingContentAsync(cancellationToken);

            _logger.LogInformation(
                "Icecat sync completed. Checked: {Checked}, Fully enriched: {FullyEnriched}, Partially enriched: {PartiallyEnriched}, Full Icecat required: {FullIcecatRequired}, Not found: {NotFound}, Failed: {Failed}",
                result.Checked,
                result.FullyEnriched,
                result.PartiallyEnriched,
                result.FullIcecatRequired,
                result.NotFound,
                result.Failed);

            foreach (var warning in result.Warnings)
            {
                _logger.LogWarning("Icecat enrichment warning: {Warning}", warning);
            }
        }
    }
}
