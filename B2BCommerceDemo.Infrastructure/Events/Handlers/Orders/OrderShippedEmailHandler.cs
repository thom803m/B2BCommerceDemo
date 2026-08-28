using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Orders
{
    public class OrderShippedEmailHandler : IEventHandler<OrderShippedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public OrderShippedEmailHandler(IEmailService emailService, IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task HandleAsync(OrderShippedEvent @event)
        {
            if (string.IsNullOrWhiteSpace(@event.UserEmail))
            {
                return;
            }

            var subject = $"Your order #{@event.OrderId} has been shipped";

            var body = _templateService.BuildOrderShippedTemplate(
                @event.OrderId);

            await _emailService.SendAsync(
                to: @event.UserEmail,
                subject: subject,
                body: body,
                isHtml: true
            );
        }
    }
}
