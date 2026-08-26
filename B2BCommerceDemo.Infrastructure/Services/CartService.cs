using B2BCommerceDemo.Core.DTOs.Carts;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly ICompanyAccessValidator _companyAccessValidator;
        private readonly IPriceService _priceService;

        public CartService(AppDbContext context, ICompanyAccessValidator companyAccessValidator, IPriceService priceService)
        {
            _context = context;
            _companyAccessValidator = companyAccessValidator;
            _priceService = priceService;
        }

        public async Task<CartDto> GetCartAsync(int companyId, string userId)
        {
            await _companyAccessValidator.ValidateCompanyActiveAsync(companyId);

            var cart = await GetCartEntity(companyId, userId);

            if (cart == null)
            {
                return new CartDto 
                { 
                    CompanyId = companyId 
                };
            }

            if (!cart.Items.Any())
            {
                return new CartDto
                {
                    Id = cart.Id,
                    CompanyId = cart.CompanyId,
                    Items = new List<CartItemDto>()
                };
            }

            var productIds = cart.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            var currentPrices = await _priceService
                .GetPricesForProductsAsync(
                    productIds,
                    companyId);

            return CartMapper.Map(cart, currentPrices);
        }

        public async Task<CartDto> AddItemAsync(int companyId, string userId, CreateCartItemDto dto)
        {
            await _companyAccessValidator.ValidateCompanyActiveAsync(companyId);

            if (dto.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }

            var cart = await GetOrCreateCart(companyId, userId);

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException("Product unavailable");
            }

            var existing = cart.Items.FirstOrDefault(x => x.ProductId == dto.ProductId);

            var totalQuantity = (existing?.Quantity ?? 0) + dto.Quantity;

            if (product.AvailableStock < totalQuantity)
            {
                throw new InvalidOperationException("Not enough stock");
            }

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                var unitPrice = await _priceService.GetPriceAsync(dto.ProductId, companyId);

                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = unitPrice
                });
            }

            await _context.SaveChangesAsync();

            return await GetCartAsync(companyId, userId);
        }

        public async Task<CartDto> UpdateItemAsync(int companyId, string userId, int itemId, UpdateCartItemDto dto)
        {
            await _companyAccessValidator.ValidateCompanyActiveAsync(companyId);

            if (dto.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }

            var cart = await GetCartEntity(companyId, userId)
                ?? throw new KeyNotFoundException("Cart not found");

            var item = cart.Items.FirstOrDefault(x => x.Id == itemId)
                ?? throw new KeyNotFoundException("Cart item not found");

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException("Product unavailable");
            }

            if (dto.Quantity > product.AvailableStock)
            {
                throw new InvalidOperationException("Not enough stock");
            }

            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return await GetCartAsync(companyId, userId);
        }

        public async Task<CartDto> RemoveItemAsync(int companyId, string userId, int itemId)
        {
            await _companyAccessValidator.ValidateCompanyActiveAsync(companyId);

            var cart = await GetCartEntity(companyId, userId)
                ?? throw new KeyNotFoundException("Cart not found");

            var item = cart.Items.FirstOrDefault(x => x.Id == itemId)
                ?? throw new KeyNotFoundException("Cart item not found"); 

            cart.Items.Remove(item);

            await _context.SaveChangesAsync();

            return await GetCartAsync(companyId, userId);
        }

        private async Task<Cart?> GetCartEntity(int companyId, string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c =>
                    c.CompanyId == companyId &&
                    c.UserId == userId);
        }

        private async Task<Cart> GetOrCreateCart(int companyId, string userId)
        {
            var cart = await GetCartEntity(companyId, userId);

            if (cart != null)
            {
                return cart;
            }

            cart = new Cart
            {
                CompanyId = companyId,
                UserId = userId
            };

            _context.Carts.Add(cart);

            return cart;
        }
    }
}
