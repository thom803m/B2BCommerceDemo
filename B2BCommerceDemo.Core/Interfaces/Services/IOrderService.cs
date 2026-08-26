using B2BCommerceDemo.Core.DTOs.Common;
using B2BCommerceDemo.Core.DTOs.Orders;
using B2BCommerceDemo.Core.Models;


namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrdersAsync(int companyId, string userId);
        Task<OrderDto> GetOrderByIdAsync(int companyId, string userId, int orderId);
        Task<CreateOrderResult> CreateFromCartAsync(int companyId, string userId, string idempotencyKey);
        Task<OrderDto> UpdateStatusAsync(int orderId, OrderStatus newStatus);
        Task<PagedResult<OrderListAdminDto>> GetOrdersAdminAsync(OrderQueryParameters parameters);
        Task<OrderDto> GetOrderByIdAdminAsync(int orderId);
    }
}

