namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IClock
    {
        DateTime UtcNow { get; }
        DateTime Today { get; }
    }
}
