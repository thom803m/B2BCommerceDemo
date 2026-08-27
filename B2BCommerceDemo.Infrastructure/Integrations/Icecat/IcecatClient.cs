using B2BCommerceDemo.Core.DTOs.Integrations.Icecat;
using B2BCommerceDemo.Core.Interfaces.Integrations.Icecat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace B2BCommerceDemo.Infrastructure.Integrations.Icecat
{
    public class IcecatClient : IIcecatClient
    {
        private readonly HttpClient _httpClient;
        private readonly IcecatOptions _options;
        private readonly ILogger<IcecatClient> _logger;

        public IcecatClient(
            HttpClient httpClient,
            IOptions<IcecatOptions> options,
            ILogger<IcecatClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<IcecatProductResponse?> GetProductByBrandAndSkuAsync(string? brand, string? sku)
        {
            EnsureEnabled();

            if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(sku))
            {
                return null;
            }

            var url = BuildProductByBrandAndSkuUrl(brand, sku);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(_options.ApiToken))
            {
                request.Headers.Add("api-token", _options.ApiToken);
            }

            if (!string.IsNullOrWhiteSpace(_options.ContentToken))
            {
                request.Headers.Add("content-token", _options.ContentToken);
            }

            _logger.LogInformation(
                "Calling Icecat API by Brand/SKU. Brand: {Brand}, SKU: {Sku}, Url: {Url}",
                brand,
                sku,
                url);

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                "Icecat API response. Status: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    message: $"Icecat request failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}",
                    inner: null,
                    statusCode: response.StatusCode);
            }

            return System.Text.Json.JsonSerializer.Deserialize<IcecatProductResponse>(
                body,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private string BuildProductByBrandAndSkuUrl(string brand, string sku)
        {
            return
                $"api?lang={Uri.EscapeDataString(_options.Language)}" +
                $"&shopname={Uri.EscapeDataString(_options.Username)}" +
                $"&Brand={Uri.EscapeDataString(brand.Trim())}" +
                $"&ProductCode={Uri.EscapeDataString(sku.Trim())}" +
                $"&content=generalinfo,essentialinfo,marketingtext,featuregroups,gallery,images";
        }

        public async Task<IcecatProductResponse?> GetProductByEanAsync(string ean)
        {
            EnsureEnabled();

            if (string.IsNullOrWhiteSpace(ean))
            {
                return null;
            }

            var url = BuildProductByEanUrl(ean);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(_options.ApiToken))
            {
                request.Headers.Add("api-token", _options.ApiToken);
            }

            if (!string.IsNullOrWhiteSpace(_options.ContentToken))
            {
                request.Headers.Add("content-token", _options.ContentToken);
            }

            _logger.LogInformation("Calling Icecat API: {Url}", url);

            var response = await _httpClient.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                "Icecat API response. Status: {StatusCode}. Body: {Body}",
                response.StatusCode,
                body);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    message: $"Icecat request failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}",
                    inner: null,
                    statusCode: response.StatusCode);
            }

            return System.Text.Json.JsonSerializer.Deserialize<IcecatProductResponse>(
                body,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private string BuildProductByEanUrl(string ean)
        {
            return
                $"api?lang={Uri.EscapeDataString(_options.Language)}" +
                $"&shopname={Uri.EscapeDataString(_options.Username)}" +
                $"&GTIN={Uri.EscapeDataString(ean.Trim())}" +
                $"&content=generalinfo,essentialinfo,marketingtext,featuregroups,gallery,images";
        }

        private void EnsureEnabled()
        {
            if (!_options.Enabled)
            {
                throw new InvalidOperationException(
                    "Icecat integration is disabled in the portfolio demo.");
            }
        }
    }
}
