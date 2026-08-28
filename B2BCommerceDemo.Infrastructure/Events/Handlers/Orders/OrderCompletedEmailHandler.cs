using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Orders
{
    public class OrderCompletedEmailHandler : IEventHandler<OrderCompletedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public OrderCompletedEmailHandler(IEmailService emailService, IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task HandleAsync(OrderCompletedEvent @event)
        {
            if (string.IsNullOrWhiteSpace(@event.UserEmail))
            {
                return;
            }

            var subject = $"Order #{@event.OrderId} completed";

            var body = _templateService.BuildOrderCompletedTemplate(
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
