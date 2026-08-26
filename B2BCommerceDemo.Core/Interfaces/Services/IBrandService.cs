using B2BCommerceDemo.Core.DTOs.Brands;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(int id);
        Task<BrandDto> CreateBrandAsync(CreateBrandDto dto);
        Task<BrandDto?> UpdateBrandAsync(int id, UpdateBrandDto dto);
        Task DeleteBrandAsync(int id);
    }
}

