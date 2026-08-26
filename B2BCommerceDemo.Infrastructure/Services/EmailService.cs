using B2BCommerceDemo.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body, bool isHtml = false)
        {
            _logger.LogInformation(
                """
                EMAIL SENT
                To: {To}
                Subject: {Subject}
                IsHtml: {IsHtml}
                Body:
                {Body}
                """,
                to,
                subject,
                isHtml,
                body
            );

            return Task.CompletedTask;
        }
    }
}
