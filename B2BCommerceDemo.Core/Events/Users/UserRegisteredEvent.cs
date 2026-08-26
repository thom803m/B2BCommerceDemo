namespace B2BCommerceDemo.Core.Events.Users
{
    public class UserRegisteredEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

