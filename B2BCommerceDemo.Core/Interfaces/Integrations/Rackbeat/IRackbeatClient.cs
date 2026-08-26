using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.DTOs.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat
{
    public interface IRackbeatClient
    {
        Task<List<ProductImportDto>> GetProductsForImportAsync(CancellationToken cancellationToken = default);
        Task<Dictionary<string, string>> GetProductFieldsAsync(string productNumber, CancellationToken cancellationToken = default);
        Task<decimal> GetProductPriceAsync(string productNumber, string currency, CancellationToken cancellationToken = default);
        Task<List<PurchaseOrderImportDto>> GetExpectedDeliveriesAsync(CancellationToken cancellationToken = default);
        Task<RackbeatOrderResponse?> GetOrderAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<string?> CreateOrderAsync(Order order, string customerNumber, CancellationToken cancellationToken = default);
        Task BookOrderAsync(string orderNumber, CancellationToken cancellationToken = default);
    }
}

