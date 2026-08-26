using Microsoft.AspNetCore.Identity;

namespace B2BCommerceDemo.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}

