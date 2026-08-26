using B2BCommerceDemo.Core.Events.Users;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace B2BCommerceDemo.Infrastructure.Events.Handlers.Users
{
    public class PasswordResetRequestedEventHandler
        : IEventHandler<PasswordResetRequestedEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly IConfiguration _configuration;

        public PasswordResetRequestedEventHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IEmailTemplateService templateService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _templateService = templateService;
            _configuration = configuration;
        }

        public async Task HandleAsync(
            PasswordResetRequestedEvent @event)
        {
            var user = await _userManager.FindByIdAsync(
                @event.UserId
            );

            if (
                user == null ||
                !user.EmailConfirmed ||
                string.IsNullOrWhiteSpace(user.Email)
            )
            {
                return;
            }

            var encodedToken =
                WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(
                        @event.Token
                    )
                );

            var frontendBaseUrl =
                _configuration[
                    "Frontend:BaseUrl"
                ]?.TrimEnd('/');

            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new InvalidOperationException(
                    "Frontend:BaseUrl must be configured."
                );
            }

            var resetLink =
                $"{frontendBaseUrl}/reset-password" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={Uri.EscapeDataString(encodedToken)}";

            var body =
                _templateService
                    .BuildForgotPasswordTemplate(
                        resetLink
                    );

            await _emailService.SendAsync(
                user.Email,
                "Reset your password",
                body,
                true
            );
        }
    }
}
