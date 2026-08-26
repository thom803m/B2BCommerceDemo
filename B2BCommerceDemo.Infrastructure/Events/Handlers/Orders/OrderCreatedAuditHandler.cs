using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using Microsoft.Extensions.Logging;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Orders
{
    public class OrderCreatedAuditHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedAuditHandler> _logger;

        public OrderCreatedAuditHandler(ILogger<OrderCreatedAuditHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(OrderCreatedEvent @event)
        {
            _logger.LogInformation(
                "Order created: {OrderId}, Company: {CompanyId}, Total: {Total}",
                @event.OrderId,
                @event.CompanyId,
                @event.Total
            );

            return Task.CompletedTask;
        }
    }
}
