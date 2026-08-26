using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Core.DTOs.Categories;
using B2BCommerceDemo.Core.DTOs.Images;
using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Core.Models;

namespace B2BCommerceDemo.Infrastructure.Mappers
{
    public static class ProductMapper
    {
        private static BrandDto? MapBrand(Brand? brand)
            => brand == null
                ? null
                : new BrandDto
                {
                    Id = brand.Id,
                    Name = brand.Name
                };

        private static CategoryDto? MapCategory(Category? category)
            => category == null
                ? null
                : new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                };

        public static ProductDto MapToDto(Product product, decimal price)
        {
            return new ProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Ean = product.Ean,
                Name = !string.IsNullOrWhiteSpace(product.IcecatName)
                    ? product.IcecatName
                    : product.Name,
                BasePrice = price,
                AvailableStock = product.AvailableStock,
                PurchasedQuantity = product.PurchasedQuantity,
                ExpectedDeliveryDate = product.ExpectedDeliveryDate,
                IsActive = product.IsActive,
                IcecatName = product.IcecatName,
                Description = product.Description,
                SpecificationsJson = product.SpecificationsJson,
                IcecatProductId = product.IcecatProductId,
                IcecatLastSynced = product.IcecatLastSynced,
                ContentSource = product.ContentSource,
                ContentLocked = product.ContentLocked,
                Brand = MapBrand(product.Brand),
                Category = MapCategory(product.Category),
                Images = product.Images
                    .Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        IsPrimary = i.IsPrimary
                    })
                    .ToList()
            };
        }
    }
}
