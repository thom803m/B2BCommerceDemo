using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IProductImageService
    {
        Task<ProductImage?> UploadFromUrlAsync(int productId, string imageUrl, string source = "Manual");
        Task SetPrimaryAsync(int productId, int imageId);
        Task DeleteImageAsync(int productId, int imageId);
    }
}
