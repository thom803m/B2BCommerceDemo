using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Imports.Helpers
{
    public class ProductImportImageHandler
    {
        private readonly AppDbContext _context;
        private readonly IProductImageService _productImageService;

        public ProductImportImageHandler(
            AppDbContext context,
            IProductImageService productImageService)
        {
            _context = context;
            _productImageService = productImageService;
        }

        public async Task HandleImagesAsync(
            int productId,
            string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return;
            }

            var hasImages = await _context.ProductImages.AnyAsync(i => i.ProductId == productId);

            if (hasImages)
            {
                return;
            }

            var urls = imageUrl
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(url => url.Trim())
                .Select(GetBestRackbeatImageUrl)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var (url, index) in urls.Select((value, i) => (value, i)))
            {
                var uploaded = await _productImageService.UploadFromUrlAsync(productId, url, "Rackbeat");

                if (uploaded != null && index == 0)
                {
                    await _productImageService.SetPrimaryAsync(productId, uploaded.Id);
                }
            }
        }

        private static string GetBestRackbeatImageUrl(string imageUrl)
        {
            if (!Uri.TryCreate(
                imageUrl,
                UriKind.Absolute,
                out var uri))
            {
                return imageUrl;
            }

            if (!string.Equals(
                uri.Host,
                "cdn.rackbeat.com",
                StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            var path = uri.AbsolutePath;

            var cropPathIndex = path.LastIndexOf(
                "/c/",
                StringComparison.OrdinalIgnoreCase);

            if (cropPathIndex < 0)
            {
                return imageUrl;
            }

            var fileName = path[(cropPathIndex + 3)..];

            var extension =  Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                return imageUrl;
            }

            var displaySuffix = $"-display{extension}";

            if (!fileName.EndsWith( displaySuffix, StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            var originalFileName = fileName[
                ..^displaySuffix.Length] +
                extension;

            var uriBuilder = new UriBuilder(uri)
            {
                Path = path[..cropPathIndex] + "/" + originalFileName
            };

            return uriBuilder.Uri.ToString();
        }
    }
}
