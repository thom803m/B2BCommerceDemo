using B2BCommerceDemo.Core.Events.Users;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;


namespace B2BCommerceDemo.Infrastructure.EventHandlers.Users
{
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly IConfiguration _configuration;

        public UserRegisteredEventHandler(
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

        public async Task HandleAsync(UserRegisteredEvent @event)
        {
            var user = await _userManager.FindByIdAsync(@event.UserId);

            if (user == null)
            {
                return;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontendBaseUrl = _configuration["Frontend:BaseUrl"]?.TrimEnd('/');

            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new InvalidOperationException(
                    "Frontend:BaseUrl must be configured."
                );
            }

            var confirmationLink =
                $"{frontendBaseUrl}/confirm-email" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={Uri.EscapeDataString(encodedToken)}";

            var body = _templateService.BuildEmailConfirmationTemplate(confirmationLink);

            await _emailService.SendAsync(
                user.Email!,
                "Confirm your email",
                body,
                true);
        }
    }
}
