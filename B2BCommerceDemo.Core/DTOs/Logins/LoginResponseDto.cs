namespace B2BCommerceDemo.Core.DTOs.Logins
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int? CompanyId { get; set; }
    }
}
