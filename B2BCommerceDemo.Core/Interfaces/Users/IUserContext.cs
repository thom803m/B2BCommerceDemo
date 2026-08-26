namespace B2BCommerceDemo.Core.Interfaces.Users
{
    public interface IUserContext
    {
        string? UserId { get; }
        int? CompanyId { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}

