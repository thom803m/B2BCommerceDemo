using B2BCommerceDemo.Core.Events.Users;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;


namespace B2BCommerceDemo.Infrastructure.EventHandlers.Users
{
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public UserRegisteredEventHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _templateService = templateService;
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

            var confirmationLink =
                $"https://localhost:7160/api/accounts/confirm-email" +
                $"?userId={user.Id}" +
                $"&token={encodedToken}";

            var body = _templateService.BuildEmailConfirmationTemplate(confirmationLink);

            await _emailService.SendAsync(
                user.Email!,
                "Confirm your email",
                body,
                true);
        }
    }
}
