namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IPriceService
    {
        Task<decimal> GetPriceAsync(int productId, int companyId);
        Task<Dictionary<int, decimal>> GetPricesForProductsAsync(List<int> productIds, int companyId);
    }
}

