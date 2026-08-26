using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly AppDbContext _context;

        public ProductImageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductImage?> UploadFromUrlAsync(
            int productId, 
            string imageUrl, 
            string source = "Manual")
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Image URL is required");
            }

            var normalizedUrl = imageUrl.Trim();

            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with id {productId} was not found.");
            }

            var existingImage = product.Images
                .FirstOrDefault(i => i.Url == normalizedUrl);

            if (existingImage != null)
            {
                return existingImage;
            }

            var image = new ProductImage
            {
                ProductId = productId,
                Url = normalizedUrl,
                IsPrimary = !product.Images.Any(),
                Source = source,
                ExternalId = source == "Rackbeat" ? normalizedUrl : null,
                LastSynced = source == "Rackbeat" ? DateTime.UtcNow : null
            };

            _context.ProductImages.Add(image);

            await _context.SaveChangesAsync();

            return image;
        }

        public async Task SetPrimaryAsync(int productId, int imageId)
        {
            var images = await _context.ProductImages
                .Where(image => image.ProductId == productId)
                .ToListAsync();

            var selectedImage = images.FirstOrDefault(image => image.Id == imageId);

            if (selectedImage == null)
            {
                throw new KeyNotFoundException(
                    $"Image with id {imageId} was not found for product {productId}."
                );
            }

            if (selectedImage.IsPrimary)
            {
                return;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var image in images)
                {
                    image.IsPrimary = false;
                }

                await _context.SaveChangesAsync();

                selectedImage.IsPrimary = true;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteImageAsync(int productId, int imageId)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(i =>
                    i.Id == imageId &&
                    i.ProductId == productId);

            if (image == null)
            {
                throw new KeyNotFoundException($"Image with id {imageId} was not found.");
            }

            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.Id)
                .ToListAsync();

            ProductImage? nextPrimary = null;

            if (image.IsPrimary)
            {
                nextPrimary = images
                    .FirstOrDefault(i => i.Id != imageId);
            }

            _context.ProductImages.Remove(image);

            if (nextPrimary != null)
            {
                nextPrimary.IsPrimary = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
