namespace B2BCommerceDemo.Core.DTOs.Logins
{
    public class ResetPasswordDto
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}

