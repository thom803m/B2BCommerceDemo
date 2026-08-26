using B2BCommerceDemo.Core.DTOs.Integrations.Icecat;
using B2BCommerceDemo.Core.DTOs.Products;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IProductContentEnrichmentService
    {
        Task<ProductDto?> EnrichProductAsync(int productId);
        Task<IcecatEnrichmentResult> EnrichMissingContentAsync(CancellationToken cancellationToken = default);
    }
}
