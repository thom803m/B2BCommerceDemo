using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using Microsoft.Extensions.Logging;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Orders
{
    public class OrderStatusChangedAuditHandler : IEventHandler<OrderStatusChangedEvent>
    {
        private readonly ILogger<OrderStatusChangedAuditHandler> _logger;

        public OrderStatusChangedAuditHandler(ILogger<OrderStatusChangedAuditHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(OrderStatusChangedEvent @event)
        {
            _logger.LogInformation(
                "Order {OrderId} changed status from {Old} to {New}",
                @event.OrderId,
                @event.OldStatus,
                @event.NewStatus
            );

            return Task.CompletedTask;
        }
    }
}
