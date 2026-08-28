using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Orders
{
    public class OrderCancelledEmailHandler : IEventHandler<OrderCancelledEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public OrderCancelledEmailHandler(IEmailService emailService, IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task HandleAsync(OrderCancelledEvent @event)
        {
            if (string.IsNullOrWhiteSpace(@event.UserEmail))
            {
                return;
            }

            var subject = $"Order #{@event.OrderId} cancelled";

            var body = _templateService.BuildOrderCancelledTemplate(
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
