using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Infrastructure.Services.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailOptions _options;

        public EmailService(
            ILogger<EmailService> logger,
            IOptions<EmailOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendAsync(
            string to,
            string subject,
            string body,
            bool isHtml = false)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation(
                    "Email delivery is disabled. Message to {To} with subject {Subject} was not sent.",
                    to,
                    subject);

                return;
            }

            ValidateConfiguration();

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _options.FromName,
                    _options.FromEmail));

            message.To.Add(MailboxAddress.Parse(to));

            message.Subject = subject;

            message.Body = new TextPart(
                isHtml ? "html" : "plain")
            {
                Text = body
            };

            using var client = new SmtpClient();

            var socketOptions = _options.UseSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                socketOptions);

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await client.AuthenticateAsync(
                    _options.Username,
                    _options.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation(
                "Email sent successfully to {To} with subject {Subject}.",
                to,
                subject);
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_options.Host))
            {
                throw new InvalidOperationException(
                    "Email SMTP host is not configured.");
            }

            if (_options.Port <= 0)
            {
                throw new InvalidOperationException(
                    "Email SMTP port is invalid.");
            }

            if (string.IsNullOrWhiteSpace(_options.FromEmail))
            {
                throw new InvalidOperationException(
                    "Email sender address is not configured.");
            }
        }
    }
}