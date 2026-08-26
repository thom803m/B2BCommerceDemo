using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IJwtService
    {
        Task<string> GenerateToken(ApplicationUser user, int? companyId);
    }
}

