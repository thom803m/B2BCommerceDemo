using B2BCommerceDemo.Core.DTOs.Integrations.Icecat;

namespace B2BCommerceDemo.Core.Interfaces.Integrations.Icecat
{
    public interface IIcecatClient
    {
        Task<IcecatProductResponse?> GetProductByEanAsync(string ean);

        Task<IcecatProductResponse?> GetProductByBrandAndSkuAsync(string? brand, string? sku);
    }
}
