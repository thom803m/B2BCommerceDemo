using B2BCommerceDemo.Core.DTOs.Orders;
using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Infrastructure.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto Map(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CompanyId = order.CompanyId,
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Status = order.Status.ToString(),
                RackbeatOrderNumber = order.RackbeatOrderNumber,
                RackbeatSyncStatus = order.RackbeatSyncStatus.ToString(),
                RackbeatSyncError = order.RackbeatSyncError,
                RackbeatSyncedAt = order.RackbeatSyncedAt,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Sku = i.Sku,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.Quantity * i.UnitPrice
                }).ToList()
            };
        }
    }
}
