using B2BCommerceDemo.Core.DTOs.Carts;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int companyId, string userId);
        Task<CartDto> AddItemAsync(int companyId, string userId, CreateCartItemDto dto);
        Task<CartDto> UpdateItemAsync(int companyId, string userId, int itemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveItemAsync(int companyId, string userId, int itemId);
    }
}
