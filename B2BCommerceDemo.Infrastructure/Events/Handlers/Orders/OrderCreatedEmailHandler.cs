using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Orders
{
    public class OrderCreatedEmailHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public OrderCreatedEmailHandler(IEmailService emailService, IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            if (string.IsNullOrWhiteSpace(@event.UserEmail))
                return;

            var subject = $"Order #{@event.OrderId} confirmed";

            var body = _templateService.BuildOrderCreatedTemplate(
                @event.OrderId,
                @event.Total,
                @event.CreatedAt);

            await _emailService.SendAsync(
                to: @event.UserEmail!,
                subject: subject,
                body: body,
                isHtml: true
            );
        }
    }
}
