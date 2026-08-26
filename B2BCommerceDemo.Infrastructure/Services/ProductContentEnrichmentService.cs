using B2BCommerceDemo.Core.DTOs.Integrations.Icecat;
using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Core.Interfaces.Integrations.Icecat;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class ProductContentEnrichmentService : IProductContentEnrichmentService
    {
        private readonly AppDbContext _context;
        private readonly IIcecatClient _icecatClient;
        private readonly ILogger<ProductContentEnrichmentService> _logger;

        public ProductContentEnrichmentService(
            AppDbContext context,
            IIcecatClient icecatClient,
            ILogger<ProductContentEnrichmentService> logger)
        {
            _context = context;
            _icecatClient = icecatClient;
            _logger = logger;
        }

        public async Task<ProductDto?> EnrichProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            if (product.ContentLocked)
            {
                return ProductMapper.MapToDto(product, product.BasePrice);
            }

            var icecat =
                await _icecatClient
                    .GetProductByBrandAndSkuAsync(
                        product.Brand?.Name,
                        product.Sku
                    );

            if (
                !HasUsableIcecatContent(icecat) &&
                !string.IsNullOrWhiteSpace(product.Ean)
            )
            {
                _logger.LogInformation(
                    "No usable Icecat content by Brand/SKU for product {ProductId}. Trying EAN {Ean}.",
                    product.Id,
                    product.Ean
                );

                icecat =
                    await _icecatClient
                        .GetProductByEanAsync(
                            product.Ean
                        );
            }

            _logger.LogInformation(
                "Icecat response for product {ProductId}: {Json}",
                product.Id,
                JsonSerializer.Serialize(
                    icecat,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    }
                )
            );

            if (!HasUsableIcecatContent(icecat))
            {
                return ProductMapper.MapToDto(
                    product,
                    product.BasePrice
                );
            }

            if (icecat?.Data == null)
            {
                return ProductMapper.MapToDto(product, product.BasePrice);
            }

            var icecatName =
                icecat.Data.GeneralInfo
                    ?.Title
                    ?.Trim();

            if (string.IsNullOrWhiteSpace(icecatName))
            {
                icecatName =
                    icecat.Data.GeneralInfo
                        ?.TitleInfo
                        ?.BrandLocalTitle
                        ?.Value
                        ?.Trim();
            }

            if (string.IsNullOrWhiteSpace(icecatName))
            {
                icecatName =
                    icecat.Data.GeneralInfo
                        ?.TitleInfo
                        ?.GeneratedLocalTitle
                        ?.Value
                        ?.Trim();
            }

            if (string.IsNullOrWhiteSpace(icecatName))
            {
                icecatName =
                    icecat.Data.GeneralInfo
                        ?.TitleInfo
                        ?.GeneratedIntTitle
                        ?.Trim();
            }

            var description =
                icecat.Data.SummaryDescription?.SummaryDescription
                ?? icecat.Data.MarketingText;

            var specificationsJson = BuildSpecificationsJson(icecat.Data.FeaturesGroups);

            var hasIcecatContent =
                !string.IsNullOrWhiteSpace(icecat.Data.EssentialInfo?.ProductCode) ||
                !string.IsNullOrWhiteSpace(icecatName) ||
                !string.IsNullOrWhiteSpace(description) ||
                specificationsJson != "[]";

            if (!hasIcecatContent)
            {
                return ProductMapper.MapToDto(product, product.BasePrice);
            }

            if (!string.IsNullOrWhiteSpace(icecatName))
            {
                product.IcecatName = icecatName;
            }

            product.IcecatProductId = icecat.Data.EssentialInfo?.ProductCode;
            product.IcecatLastSynced = DateTime.UtcNow;
            product.ContentSource = "Icecat";

            product.Description = description;
            product.SpecificationsJson = specificationsJson;

            SyncIcecatImages(product, icecat.Data.Gallery);

            await _context.SaveChangesAsync();

            return ProductMapper.MapToDto(product, product.BasePrice);
        }

        private void SyncIcecatImages(Product product, List<IcecatGalleryImage> gallery)
        {
            if (!gallery.Any())
            {
                return;
            }

            var now = DateTime.UtcNow;

            var nonIcecatImagesToRemove = product.Images
                .Where(x =>
                    x.Source == "Rackbeat" ||
                    string.IsNullOrWhiteSpace(x.Source) ||
                    x.Url != null && x.Url.Contains("cdn.rackbeat.com"))
                .ToList();

            foreach (var image in nonIcecatImagesToRemove)
            {
                _context.ProductImages.Remove(image);
            }

            foreach (var image in product.Images)
            {
                image.IsPrimary = false;
            }

            var incomingExternalIds = gallery
                .Select(x => x.Id ?? GetBestImageUrl(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .ToHashSet();

            var existingIcecatImages = product.Images
                .Where(x => x.Source == "Icecat")
                .ToList();

            var removedImages = existingIcecatImages
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.ExternalId) &&
                    !incomingExternalIds.Contains(x.ExternalId))
                .ToList();

            foreach (var image in removedImages)
            {
                _context.ProductImages.Remove(image);
            }

            var primaryHasBeenSet = false;

            foreach (var image in gallery)
            {
                var url = GetBestImageUrl(image);

                if (string.IsNullOrWhiteSpace(url))
                {
                    continue;
                }

                var externalId = image.Id ?? url.Trim();

                var isMain =
                    image.IsMain == "Y" ||
                    image.IsMain == "1" ||
                    image.IsMain?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;

                var shouldBePrimary = isMain || !primaryHasBeenSet;

                var existing = product.Images.FirstOrDefault(x =>
                    x.Source == "Icecat" &&
                    x.ExternalId == externalId);

                if (existing == null)
                {
                    product.Images.Add(new ProductImage
                    {
                        Url = url.Trim(),
                        IsPrimary = shouldBePrimary,
                        Source = "Icecat",
                        ExternalId = externalId,
                        LastSynced = now
                    });
                }
                else
                {
                    existing.Url = url.Trim();
                    existing.IsPrimary = shouldBePrimary;
                    existing.LastSynced = now;
                }

                if (shouldBePrimary)
                {
                    primaryHasBeenSet = true;
                }
            }
        }

        private static bool HasUsableIcecatContent(IcecatProductResponse? response)
        {
            var data = response?.Data;

            if (data == null)
            {
                return false;
            }

            var hasName =
                !string.IsNullOrWhiteSpace(
                    data.GeneralInfo?.Title
                ) ||
                !string.IsNullOrWhiteSpace(
                    data.GeneralInfo
                        ?.TitleInfo
                        ?.BrandLocalTitle
                        ?.Value
                ) ||
                !string.IsNullOrWhiteSpace(
                    data.GeneralInfo
                        ?.TitleInfo
                        ?.GeneratedLocalTitle
                        ?.Value
                ) ||
                !string.IsNullOrWhiteSpace(
                    data.GeneralInfo
                        ?.TitleInfo
                        ?.GeneratedIntTitle
                );

            var hasDescription =
                !string.IsNullOrWhiteSpace(
                    data.SummaryDescription
                        ?.SummaryDescription
                ) ||
                !string.IsNullOrWhiteSpace(
                    data.MarketingText
                );

            var hasSpecifications =
                data.FeaturesGroups?.Any(
                    group =>
                        group.Features?.Any() ==
                        true
                ) == true;

            var hasImages =
                data.Gallery?.Any() == true;

            var hasProductCode =
                !string.IsNullOrWhiteSpace(
                    data.EssentialInfo?.ProductCode
                );

            return
                hasName ||
                hasDescription ||
                hasSpecifications ||
                hasImages ||
                hasProductCode;
        }

        private static string BuildSpecificationsJson(List<IcecatFeaturesGroup> featureGroups)
        {
            var groups = featureGroups
                .Select(group => new IcecatProductSpecificationGroupDto
                {
                    GroupName = group.FeatureGroup?.Name?.Value ?? "",
                    Items = group.Features
                        .Where(feature =>
                            !string.IsNullOrWhiteSpace(feature.Feature?.Name?.Value) &&
                            !string.IsNullOrWhiteSpace(feature.PresentationValue))
                        .Select(feature => new IcecatProductSpecificationItemDto
                        {
                            Name = feature.Feature!.Name!.Value!,
                            Value = feature.PresentationValue!
                        })
                        .ToList()
                })
                .Where(group =>
                    !string.IsNullOrWhiteSpace(group.GroupName) &&
                    group.Items.Any())
                .ToList();

            return System.Text.Json.JsonSerializer.Serialize(groups);
        }

        public async Task<IcecatEnrichmentResult> EnrichMissingContentAsync(CancellationToken cancellationToken = default)
        {
            var result = new IcecatEnrichmentResult();

            var products = await _context.Products
                .AsNoTracking()
                .Where(p =>
                    p.IsActive &&
                    !string.IsNullOrWhiteSpace(p.Sku) &&
                    p.Brand != null &&
                    !string.IsNullOrWhiteSpace(p.Brand.Name) &&
                    !p.ContentLocked &&
                    (
                        string.IsNullOrWhiteSpace(p.IcecatName) ||
                        string.IsNullOrWhiteSpace(p.Description) ||
                        string.IsNullOrWhiteSpace(p.SpecificationsJson) ||
                        p.Images.Count == 0
                    ))
                .OrderBy(p => p.Id)
                .Take(1000)
                .Select(p => new
                {
                    p.Id,
                    p.Sku,
                    BrandName = p.Brand!.Name
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Icecat enrichment found {Count} products to check.",
                products.Count);

            result.Checked = products.Count;

            foreach (var product in products)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var enrichedProduct = await EnrichProductAsync(product.Id);

                    var matchedIcecat = enrichedProduct?.ContentSource == "Icecat";

                    if (!matchedIcecat)
                    {
                        result.NotFound++;

                        result.Warnings.Add(
                            $"Product {product.Id} / SKU {product.Sku} / Brand {product.BrandName}: No Icecat content found.");

                        continue;
                    }

                    var hasIcecatName = !string.IsNullOrWhiteSpace(enrichedProduct.IcecatName);

                    var hasDescription = !string.IsNullOrWhiteSpace(enrichedProduct.Description);

                    var hasSpecifications = !string.IsNullOrWhiteSpace(
                        enrichedProduct.SpecificationsJson) &&
                        enrichedProduct.SpecificationsJson != "[]";

                    var hasImages = enrichedProduct.Images.Any();

                    if (hasIcecatName &&
                        hasDescription &&
                        hasSpecifications &&
                        hasImages)
                    {
                        result.FullyEnriched++;
                    }
                    else
                    {
                        result.PartiallyEnriched++;

                        var missingParts = new List<string>();

                        if (!hasIcecatName)
                        {
                            missingParts.Add("product name");
                        }

                        if (!hasDescription)
                        {
                            missingParts.Add("description");
                        }

                        if (!hasSpecifications)
                        {
                            missingParts.Add("specifications");
                        }

                        if (!hasImages)
                        {
                            missingParts.Add("images");
                        }

                        result.Warnings.Add(
                            $"Product {product.Id} / SKU {product.Sku} / Brand {product.BrandName}: Icecat content was found, but the product is still missing {string.Join(", ", missingParts)}.");
                    }
                }
                catch (HttpRequestException ex)
                    when (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    result.FullIcecatRequired++;

                    result.Warnings.Add(
                        $"Product {product.Id} / SKU {product.Sku} / Brand {product.BrandName}: Full Icecat access is required.");

                    _logger.LogWarning(
                        ex,
                        "Full Icecat access required for product {ProductId}, SKU {Sku}, Brand {Brand}",
                        product.Id,
                        product.Sku,
                        product.BrandName);
                }
                catch (HttpRequestException ex)
                {
                    result.Failed++;

                    result.Warnings.Add(
                        $"Product {product.Id} / SKU {product.Sku} / Brand {product.BrandName}: Icecat request failed - {ex.Message}");

                    _logger.LogError(
                        ex,
                        "Icecat request failed for product {ProductId}, SKU {Sku}, Brand {Brand}",
                        product.Id,
                        product.Sku,
                        product.BrandName);
                }
                catch (Exception ex)
                {
                    result.Failed++;

                    result.Warnings.Add(
                        $"Product {product.Id} / SKU {product.Sku} / Brand {product.BrandName}: Icecat enrichment failed - {ex.Message}");

                    _logger.LogError(
                        ex,
                        "Icecat enrichment failed for product {ProductId}, SKU {Sku}, Brand {Brand}",
                        product.Id,
                        product.Sku,
                        product.BrandName);
                }
            }

            return result;
        }

        private static string? GetBestImageUrl(IcecatGalleryImage image)
        {
            var candidates = new[]
            {
                image.Pic500x500,
                image.Pic,
                image.LowPic,
                image.ThumbPic,
                image.HighPic,
                image.Original
            };

            return candidates.FirstOrDefault(url => !string.IsNullOrWhiteSpace(url));
        }
    }
}
