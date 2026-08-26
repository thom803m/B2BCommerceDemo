using B2BCommerceDemo.Core.Interfaces.Users;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace B2BCommerceDemo.Infrastructure.Users
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? UserId =>
            User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public int? CompanyId
        {
            get
            {
                var value = User?.FindFirst("CompanyId")?.Value;
                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public bool IsAdmin =>
            User?.IsInRole("Admin") ?? false;
    }
}

