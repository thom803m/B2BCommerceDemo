using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using Microsoft.Extensions.Logging;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Orders.Rackbeat
{
    public class OrderCreatedRackbeatHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly IRackbeatOrderSyncService _rackbeatOrderSyncService;
        private readonly ILogger<OrderCreatedRackbeatHandler> _logger;

        public OrderCreatedRackbeatHandler(
            IRackbeatOrderSyncService rackbeatOrderSyncService,
            ILogger<OrderCreatedRackbeatHandler> logger)
        {
            _rackbeatOrderSyncService = rackbeatOrderSyncService;
            _logger = logger;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            var result = await _rackbeatOrderSyncService.SyncOrderAsync(@event.OrderId);

            _logger.LogInformation(
                "Rackbeat order sync completed for order {OrderId}. Created: {Created}, Skipped: {Skipped}, Warnings: {Warnings}",
                @event.OrderId,
                result.Created,
                result.Skipped,
                result.Warnings.Count);

            foreach (var warning in result.Warnings)
            {
                _logger.LogWarning(
                    "Rackbeat order sync warning for order {OrderId}: {Warning}",
                    @event.OrderId,
                    warning);
            }
        }
    }
}
