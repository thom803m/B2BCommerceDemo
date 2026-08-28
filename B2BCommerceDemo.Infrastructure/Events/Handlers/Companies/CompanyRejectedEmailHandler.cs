using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Companies
{
    public class CompanyRejectedEmailHandler
        : IEventHandler<CompanyRejectedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public CompanyRejectedEmailHandler(
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task HandleAsync(CompanyRejectedEvent @event)
        {
            var html = _templateService
                .BuildCompanyRejectedTemplate(@event.CompanyName);

            await _emailService.SendAsync(
                @event.UserEmail,
                "Company registration rejected",
                html,
                true);
        }
    }
}
