namespace B2BCommerceDemo.Core.Events.Users
{
    public class PasswordResetRequestedEvent
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
