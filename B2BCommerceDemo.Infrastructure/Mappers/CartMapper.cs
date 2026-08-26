using B2BCommerceDemo.Core.DTOs.Carts;
using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Infrastructure.Mappers
{
    public static class CartMapper
    {
        public static CartDto Map(Cart cart, Dictionary<int, decimal> currentPrices)
        {
            return new CartDto
            {
                Id = cart.Id,
                CompanyId = cart.CompanyId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    ImageUrl = i.Product?.Images?
                        .FirstOrDefault(img => img.IsPrimary)?.Url,
                    Quantity = i.Quantity,
                    UnitPrice = currentPrices.TryGetValue(
                        i.ProductId,
                        out var currentPrice)
                            ? currentPrice
                            : i.UnitPrice
                }).ToList()
            };
        }
    }
}

