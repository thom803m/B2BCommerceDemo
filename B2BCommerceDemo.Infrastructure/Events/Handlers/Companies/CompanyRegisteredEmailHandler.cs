using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Companies
{
    public class CompanyRegisteredEmailHandler
        : IEventHandler<CompanyRegisteredEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public CompanyRegisteredEmailHandler(
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task HandleAsync(CompanyRegisteredEvent @event)
        {
            var html = _templateService
                .BuildCompanyRegisteredTemplate(@event.CompanyName);

            await _emailService.SendAsync(
                @event.UserEmail,
                "Company registration received",
                html);
        }
    }
}
