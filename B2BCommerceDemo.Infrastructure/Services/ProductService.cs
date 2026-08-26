using B2BCommerceDemo.Core.DTOs.Common;
using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class ProductService : IProductService 
    {
        private readonly AppDbContext _context;
        private readonly IValidateUniqueness _validateUniqueness;
        private readonly IClock _clock;
        private readonly IPriceService _priceService;

        public ProductService(AppDbContext context, IValidateUniqueness validateUniqueness, IClock clock, IPriceService priceService)
        {
            _context = context;
            _validateUniqueness = validateUniqueness;
            _clock = clock;
            _priceService = priceService;
        }

        private IQueryable<Product> ProductsQuery() 
        {
            return _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images);
        }

        // IProductService implementation
        public async Task<List<ProductDto>> GetAllProductsAsync(int? companyId, bool isAdmin)
        {
            var query = ProductsQuery()
                .AsNoTracking();

            if (!isAdmin)
            {
                query = query.Where(p => p.IsActive);
            }

            var products = await query
                .OrderBy(p => p.Id)
                .ToListAsync();

            Dictionary<int, decimal> prices = new();

            if (!isAdmin && companyId.HasValue)
            {
                var productIds = products.Select(p => p.Id).ToList();

                prices = await _priceService.GetPricesForProductsAsync(productIds, companyId.Value);
            }

            return products.Select(product =>
            {
                var price = product.BasePrice;

                if (!isAdmin && companyId.HasValue && prices.TryGetValue(product.Id, out var companyPrice))
                {
                    price = companyPrice;
                }

                return ProductMapper.MapToDto(product, price);
            }).ToList();
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters, int? companyId, bool isAdmin)
        {
            var page = parameters.Page <= 0 ? 1 : parameters.Page; 
            var pageSize = parameters.PageSize <= 0 ? 1000 : parameters.PageSize; 
            
            var query = ProductsQuery().AsNoTracking(); 
            
            if (!isAdmin) 
            { 
                query = query.Where(p => p.IsActive); 
            }

            if (!string.IsNullOrWhiteSpace(parameters.Search)) 
            { 
                var search = $"%{parameters.Search.Trim()}%"; 
                
                query = query.Where(p => 
                    EF.Functions.Like(p.Name, search) ||
                    EF.Functions.Like(p.IcecatName, search) ||
                    EF.Functions.Like(p.Sku, search) || 
                    EF.Functions.Like(p.Ean, search) || 
                    (p.Brand != null && EF.Functions.Like(p.Brand.Name, search)) || 
                    (p.Category != null && EF.Functions.Like(p.Category.Name, search))); 
            }

            if (!string.IsNullOrWhiteSpace(parameters.Sku))
            {
                var sku = parameters.Sku.Trim();

                query = query.Where(p => p.Sku == sku);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Ean))
            {
                var ean = parameters.Ean.Trim();

                query = query.Where(p => p.Ean == ean);
            }

            if (parameters.InStock.HasValue)
            {
                query = parameters.InStock.Value
                    ? query.Where(p => p.AvailableStock > 0)
                    : query.Where(p => p.AvailableStock <= 0);
            }

            if (parameters.IsPurchased.HasValue)
            {
                query = parameters.IsPurchased.Value
                    ? query.Where(p => p.PurchasedQuantity > 0)
                    : query.Where(p => p.PurchasedQuantity <= 0);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Brand)) 
            {
                var brandSearch = $"%{parameters.Brand.Trim()}%";

                query = query.Where(p => 
                    p.Brand != null &&
                    EF.Functions.Like(p.Brand.Name, brandSearch));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Category)) 
            {
                var categorySearch = $"%{parameters.Category.Trim()}%";

                query = query.Where(p => 
                    p.Category != null &&
                    EF.Functions.Like(p.Category.Name, categorySearch));
            }

            if (!string.IsNullOrWhiteSpace(parameters.ContentSource))
            {
                var contentSource = parameters.ContentSource.Trim();

                query = query.Where(p => p.ContentSource == contentSource);
            }

            if (parameters.ContentLocked.HasValue)
            {
                query = query.Where(p => p.ContentLocked == parameters.ContentLocked.Value);
            }

            if (parameters.HasIcecatProductId.HasValue)
            {
                if (parameters.HasIcecatProductId.Value)
                {
                    query = query.Where(p => !string.IsNullOrWhiteSpace(p.IcecatProductId));
                }
                else
                {
                    query = query.Where(p => string.IsNullOrWhiteSpace(p.IcecatProductId));
                }
            }

            if (parameters.HasContent.HasValue)
            {
                if (parameters.HasContent.Value)
                {
                    query = query.Where(p =>
                        !string.IsNullOrWhiteSpace(p.Description) ||
                        !string.IsNullOrWhiteSpace(p.SpecificationsJson));
                }
                else
                {
                    query = query.Where(p =>
                        string.IsNullOrWhiteSpace(p.Description) &&
                        string.IsNullOrWhiteSpace(p.SpecificationsJson));
                }
            }

            if (parameters.MinPrice.HasValue) 
            {
                query = query.Where(p => 
                    p.BasePrice >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue) 
            { 
                query = query.Where(p => 
                    p.BasePrice <= parameters.MaxPrice.Value); 
            }

            var totalCount = await query.CountAsync();

            var sortDescending = string.Equals(
                parameters.SortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

            query = parameters.SortBy?.Trim().ToLowerInvariant() switch
            {
                "price" => sortDescending
                    ? query.OrderByDescending(p => p.BasePrice).ThenBy(p => p.Id)
                    : query.OrderBy(p => p.BasePrice).ThenBy(p => p.Id),

                "stock" => sortDescending
                    ? query.OrderByDescending(p => p.AvailableStock).ThenBy(p => p.Id)
                    : query.OrderBy(p => p.AvailableStock).ThenBy(p => p.Id),

                "name" => sortDescending
                    ? query.OrderByDescending(p => p.IcecatName ?? p.Name).ThenBy(p => p.Id)
                    : query.OrderBy(p => p.IcecatName ?? p.Name).ThenBy(p => p.Id),

                _ => query.OrderBy(p => p.Name).ThenBy(p => p.Id)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDtos = new List<ProductDto>();

            Dictionary<int, decimal> prices = new();

            if (!isAdmin && companyId.HasValue)
            {
                var productIds = items.Select(p => p.Id).ToList();

                prices = await _priceService.GetPricesForProductsAsync(productIds, companyId.Value);
            }

            foreach (var product in items)
            {
                decimal price = product.BasePrice;

                if (!isAdmin && companyId.HasValue && prices.TryGetValue(product.Id, out var companyPrice))
                {
                    price = companyPrice;
                }

                productDtos.Add(ProductMapper.MapToDto(product, price));
            }

            return new PagedResult<ProductDto> 
            {
                Items = productDtos,
                TotalCount = totalCount, 
                Page = page, 
                PageSize = pageSize 
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id, int? companyId, bool isAdmin)
        {
            var product = await ProductsQuery().
                AsNoTracking().
                FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || (!isAdmin && !product.IsActive))
            {
                throw new KeyNotFoundException("Product not found");
            }

            decimal price = product.BasePrice;

            if (!isAdmin && companyId.HasValue)
            {
                price = await _priceService.GetPriceAsync(
                    product.Id,
                    companyId.Value);
            }

            return ProductMapper.MapToDto(product, price);
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductDto dto)
        {
            await _validateUniqueness.ValidateUniqueSkuAsync(dto.Sku);
            await _validateUniqueness.ValidateUniqueEanAsync(dto.Ean);

            var brand = await _context.Brands.FindAsync(dto.BrandId);
            
            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            } 

            var product = new Product
            {
                Sku = dto.Sku.Trim().ToUpperInvariant(),
                Name = dto.Name.Trim(),
                BasePrice = dto.BasePrice,
                Ean = dto.Ean?.Trim(),
                AvailableStock = dto.AvailableStock,
                IsActive = dto.AvailableStock > 0,
                Brand = brand,
                Category = category,
                LastSynced = _clock.UtcNow
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return ProductMapper.MapToDto(
                product,
                product.BasePrice);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        { 
            var product = await ProductsQuery()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            await _validateUniqueness.ValidateUniqueSkuAsync(dto.Sku, product.Id);
            await _validateUniqueness.ValidateUniqueEanAsync(dto.Ean, product.Id);

            var brand = await _context.Brands.FindAsync(dto.BrandId);

            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            product.Sku = dto.Sku.Trim().ToUpperInvariant();
            product.Name = dto.Name.Trim();
            product.BasePrice = dto.BasePrice;
            product.Ean = dto.Ean?.Trim();
            product.Brand = brand;
            product.Category = category;

            await _context.SaveChangesAsync();

            return ProductMapper.MapToDto(
                product,
                product.BasePrice);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.
                FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
        }

        // Icecat integration methods
        public async Task<ProductDto?> UpdateProductContentAsync(int id, UpdateProductContentDto dto)
        {
            var product = await ProductsQuery()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            product.Description = dto.Description?.Trim();
            product.SpecificationsJson = dto.SpecificationsJson;
            product.ContentLocked = dto.ContentLocked;
            product.ContentSource = "Manual";

            await _context.SaveChangesAsync();

            return ProductMapper.MapToDto(product, product.BasePrice);
        }
    }
}

