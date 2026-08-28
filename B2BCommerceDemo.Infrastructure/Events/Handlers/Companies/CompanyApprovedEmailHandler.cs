using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Companies
{
    public class CompanyApprovedEmailHandler
        : IEventHandler<CompanyApprovedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public CompanyApprovedEmailHandler(
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task HandleAsync(CompanyApprovedEvent @event)
        {
            var html = _templateService
                .BuildCompanyApprovedTemplate(@event.CompanyName);

            await _emailService.SendAsync(
                @event.UserEmail,
                "Company approved",
                html,
                true);
        }
    }
}
