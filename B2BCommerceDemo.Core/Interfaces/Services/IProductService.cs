using B2BCommerceDemo.Core.DTOs.Common;
using B2BCommerceDemo.Core.DTOs.Products;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IProductService
    {
        // Product management methods
        Task<List<ProductDto>> GetAllProductsAsync(int? companyId, bool isAdmin);
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters, int? companyId, bool isAdmin);
        Task<ProductDto?> GetProductByIdAsync(int id, int? companyId, bool isAdmin);
        Task<ProductDto?> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task DeleteProductAsync(int id);

        // Icecat integration methods
        Task<ProductDto?> UpdateProductContentAsync(int id, UpdateProductContentDto dto);
    }
}

