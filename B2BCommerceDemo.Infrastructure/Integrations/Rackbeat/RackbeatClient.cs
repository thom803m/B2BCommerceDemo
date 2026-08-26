using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.DTOs.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatClient : IRackbeatClient
    {
        private readonly HttpClient _httpClient;

        public RackbeatClient(HttpClient httpClient, IOptions<RackbeatOptions> options)
        {
            _httpClient = httpClient;

            var rackbeatOptions = options.Value;

            _httpClient.BaseAddress = new Uri(rackbeatOptions.BaseUrl);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rackbeatOptions.ApiKey);

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Dictionary<string, string>> GetProductFieldsAsync(string productNumber, CancellationToken cancellationToken = default)
        {
            var encodedProductNumber = Uri.EscapeDataString(productNumber);

            var response = await _httpClient.GetAsync($"products/{encodedProductNumber}/fields", cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Could not fetch fields for {productNumber}: {response.StatusCode}");
                Console.WriteLine($"WARNING: No sales_price returned for {productNumber}");

                Console.WriteLine(body);

                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            using var json = JsonDocument.Parse(body);

            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!json.RootElement.TryGetProperty("field_values", out var fieldValuesElement))
            {
                Console.WriteLine($"No 'field_values' property found for {productNumber}");
                return fields;
            }

            foreach (var fieldValue in fieldValuesElement.EnumerateArray())
            {
                if (!fieldValue.TryGetProperty("field", out var fieldElement))
                {
                    continue;
                }

                var name = fieldElement.TryGetProperty("name", out var nameElement)
                    ? nameElement.GetString()
                    : null;

                var value = fieldValue.TryGetProperty("value", out var valueElement)
                    ? valueElement.GetString()
                    : null;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    fields[name] = value ?? "";
                }
            }

            return fields;
        }

        public async Task<decimal> GetProductPriceAsync(string productNumber, string currency, CancellationToken cancellationToken = default)
        {
            var encodedProductNumber = Uri.EscapeDataString(productNumber);
            var encodedCurrency = Uri.EscapeDataString(currency);

            var response = await _httpClient.GetAsync(
                $"products/{encodedProductNumber}/prices/{encodedCurrency}",
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"Could not fetch {currency} price for {productNumber}: {response.StatusCode}");

                Console.WriteLine(body);

                return 0;
            }

            using var json = JsonDocument.Parse(body);

            if (json.RootElement.TryGetProperty("currency_price", out var currencyPrice) &&
                currencyPrice.TryGetProperty("sales_price", out var salesPrice) &&
                salesPrice.TryGetDecimal(out var result))
            {
                return result;
            }

            Console.WriteLine($"WARNING: No sales_price returned for product {productNumber} ({currency})");

            return 0;
        }

        public async Task<List<ProductImportDto>> GetProductsForImportAsync(CancellationToken cancellationToken = default)
        {
            var products = new List<ProductImportDto>();

            var page = 1;
            var totalPages = 1;

            do
            {
                var response = await _httpClient.GetAsync(
                    $"products?page={page}&limit=100",
                    cancellationToken);

                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                Console.WriteLine($"Products page {page}: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                using var json = JsonDocument.Parse(body);

                if (json.RootElement.TryGetProperty("pages", out var pagesElement)
                    && pagesElement.TryGetInt32(out var parsedPages))
                {
                    totalPages = parsedPages;
                }

                foreach (var item in json.RootElement.GetProperty("products").EnumerateArray())
                {
                    var sku = item.GetProperty("number").GetString() ?? "";
                    var availableStock = GetInt(item, "available_quantity");
                    var purchasedQuantity = GetInt(item, "purchased_quantity");
                    var hasStockOrPurchased = availableStock > 0 || purchasedQuantity > 0;

                    if (!hasStockOrPurchased)
                    {
                        Console.WriteLine($"Skipped {sku}: Stock={availableStock}, Purchased={purchasedQuantity}");

                        continue;
                    }

                    var fields = await GetProductFieldsAsync(sku, cancellationToken);

                    var basePrice = GetDecimalField(fields, "EUR");

                    if (basePrice <= 0)
                    {
                        basePrice = GetDecimalField(fields, "Sales price (EUR)");
                    }

                    if (basePrice <= 0)
                    {
                        basePrice = GetDecimal(item, "sales_price");
                    }

                    if (basePrice <= 0)
                    {
                        basePrice = await GetProductPriceAsync(sku, "EUR", cancellationToken);
                    }

                    if (basePrice <= 0)
                    {
                        Console.WriteLine($"Skipped {sku}: Missing price");
                        continue;
                    }

                    var dto = new ProductImportDto
                    {
                        Sku = sku,
                        Name = item.GetProperty("name").GetString() ?? "",
                        BasePrice = basePrice,
                        AvailableStock = availableStock,
                        PurchasedQuantity = purchasedQuantity,
                        Ean = NormalizeEan(GetFieldValue(fields, "EAN")),
                        Brand = GetFieldValue(fields, "Manufacturer"),
                        Category = GetFieldValue(fields, "Product Category"),
                        ImageUrl = GetImageUrl(item)
                    };

                    Console.WriteLine($"Mapped {sku}: EAN={dto.Ean}, Brand={dto.Brand}, Category={dto.Category}");

                    products.Add(dto);
                }

                page++;

            } while (page <= totalPages);

            Console.WriteLine($"Rackbeat products mapped total: {products.Count}");

            return products;
        }

        public async Task<List<PurchaseOrderImportDto>> GetExpectedDeliveriesAsync(
            CancellationToken cancellationToken = default)
        {
            var result = new List<PurchaseOrderImportDto>();

            var page = 1;
            var totalPages = 1;

            do
            {
                var response = await _httpClient.GetAsync(
                    $"purchase-orders?is_received=false&page={page}&limit=100",
                    cancellationToken);

                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                Console.WriteLine($"Purchase orders page {page}: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                using var json = JsonDocument.Parse(body);

                if (json.RootElement.TryGetProperty("pages", out var pagesElement)
                    && pagesElement.TryGetInt32(out var parsedPages))
                {
                    totalPages = parsedPages;
                }

                if (!json.RootElement.TryGetProperty("purchase_orders", out var purchaseOrders))
                {
                    Console.WriteLine("No purchase_orders property found.");
                    Console.WriteLine(body);
                    break;
                }

                foreach (var purchaseOrder in purchaseOrders.EnumerateArray())
                {
                    var number = purchaseOrder.TryGetProperty("number", out var numberElement)
                        ? GetStringValue(numberElement)
                        : null;

                    if (string.IsNullOrWhiteSpace(number))
                        continue;

                    var purchaseOrderDeliveryDate = GetDate(purchaseOrder, "preferred_delivery_date");

                    var lines = await GetNotReceivedLinesAsync(number, purchaseOrderDeliveryDate, cancellationToken);

                    result.AddRange(lines);
                }

                page++;

            } while (page <= totalPages);

            Console.WriteLine($"Expected deliveries mapped total: {result.Count}");

            return result;
        }

        private async Task<List<PurchaseOrderImportDto>> GetNotReceivedLinesAsync(
            string purchaseOrderNumber,
            DateTime? purchaseOrderDeliveryDate,
            CancellationToken cancellationToken = default)
        {
            var result = new List<PurchaseOrderImportDto>();

            var encodedNumber = Uri.EscapeDataString(purchaseOrderNumber);

            var response = await _httpClient.GetAsync(
                $"purchase-orders/{encodedNumber}/not-received",
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Could not fetch not-received lines for PO {purchaseOrderNumber}: {response.StatusCode}");
                Console.WriteLine(body);
                return result;
            }

            using var json = JsonDocument.Parse(body);

            if (!json.RootElement.TryGetProperty("unreceived_lines", out var lines))
            {
                Console.WriteLine($"No unreceived_lines found for PO {purchaseOrderNumber}");
                Console.WriteLine(body);
                return result;
            }

            foreach (var line in lines.EnumerateArray())
            {
                if (!line.TryGetProperty("item", out var item))
                {
                    continue;
                }

                var sku = item.TryGetProperty("number", out var skuElement)
                    ? GetStringValue(skuElement)
                    : null;

                if (string.IsNullOrWhiteSpace(sku))
                {
                    continue;
                }

                var deliveryDate = GetDate(line, "delivery_date");

                Console.WriteLine($"PO={purchaseOrderNumber}, SKU={sku}, DeliveryDate={deliveryDate:yyyy-MM-dd}");

                result.Add(new PurchaseOrderImportDto
                {
                    Sku = sku,
                    ExpectedDeliveryDate = deliveryDate ?? purchaseOrderDeliveryDate
                });
            }

            return result;
        }

        public async Task<RackbeatOrderResponse?> GetOrderAsync(
            string orderNumber,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                throw new ArgumentException("Rackbeat order number cannot be empty.");
            }

            var encodedOrderNumber = Uri.EscapeDataString(orderNumber);

            var response = await _httpClient.GetAsync(
                $"orders/{encodedOrderNumber}",
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            Console.WriteLine($"Rackbeat get order {orderNumber} status: {response.StatusCode}");
            Console.WriteLine(body);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            using var json = JsonDocument.Parse(body);

            if (!json.RootElement.TryGetProperty("order", out var orderElement))
            {
                throw new InvalidOperationException("Rackbeat order response did not contain an order object.");
            }

            return JsonSerializer.Deserialize<RackbeatOrderResponse>(orderElement.GetRawText());
        }

        public async Task<string?> CreateOrderAsync(Order order, string customerNumber, CancellationToken cancellationToken = default)
        {
            var request = new RackbeatOrderRequest
            {
                CustomerNumber = customerNumber,
                Lines = order.Items.Select(item => new RackbeatOrderLineRequest
                {
                    ItemNumber = item.Sku ?? "",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            var json = JsonSerializer.Serialize(
                request,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    WriteIndented = true
                });

            Console.WriteLine("Rackbeat order request:");
            Console.WriteLine(json);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("orders", content, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            Console.WriteLine($"Rackbeat create order status: {response.StatusCode}");
            Console.WriteLine("Rackbeat create order response:");
            Console.WriteLine(body);

            response.EnsureSuccessStatusCode();

            using var responseJson = JsonDocument.Parse(body);

            if (responseJson.RootElement.TryGetProperty("order", out var orderElement) &&
                orderElement.TryGetProperty("number", out var numberElement))
            {
                return numberElement.ValueKind switch
                {
                    JsonValueKind.String => numberElement.GetString(),
                    JsonValueKind.Number => numberElement.GetInt32().ToString(),
                    _ => null
                };
            }

            throw new InvalidOperationException("Rackbeat order was created, but no order number was returned.");
        }

        // Currently not used.
        public Task BookOrderAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Rackbeat book order skipped for now. Order number: {orderNumber}");

            return Task.CompletedTask;
        }

        private static DateTime? GetDate(JsonElement element, string property)
        {
            if (!element.TryGetProperty(property, out var value))
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            var text = value.GetString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var formats = new[]
            {
                "yyyy-MM-dd",
                "dd/MM/yyyy",
                "dd-MM-yyyy",
                "MM/dd/yyyy",
                "MM-dd-yyyy"
            };

            return DateTime.TryParseExact(
                text.Trim(),
                formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result)
                ? result.Date
                : null;
        }

        private static string? GetStringValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.ToString(),
                _ => null
            };
        }

        private static string? GetImageUrl(JsonElement item)
        {
            if (item.TryGetProperty("pictures", out var pictures))
            {
                if (pictures.TryGetProperty("original", out var original))
                {
                    var originalUrl = original.GetString();

                    if (!string.IsNullOrWhiteSpace(originalUrl))
                    {
                        return originalUrl;
                    }
                }

                if (pictures.TryGetProperty("large", out var large))
                {
                    var largeUrl = large.GetString();

                    if (!string.IsNullOrWhiteSpace(largeUrl))
                    {
                        return largeUrl;
                    }
                }

                if (pictures.TryGetProperty("display", out var display))
                {
                    var displayUrl = display.GetString();

                    if (!string.IsNullOrWhiteSpace(displayUrl))
                    {
                        return displayUrl;
                    }
                }
            }

            if (item.TryGetProperty("picture_url", out var pictureUrl))
            {
                var url = pictureUrl.GetString();

                if (!string.IsNullOrWhiteSpace(url))
                {
                    return url;
                }
            }

            return null;
        }

        private static string NormalizeEan(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
                
            return value.Trim();
        }

        private static decimal GetDecimal(
            JsonElement element,
            string property)
        {
            return element.TryGetProperty(property, out var value)
                   && value.TryGetDecimal(out var result)
                ? result
                : 0;
        }

        private static int GetInt(
            JsonElement element,
            string property)
        {
            return element.TryGetProperty(property, out var value)
                   && value.TryGetInt32(out var result)
                ? result
                : 0;
        }

        private static string GetFieldValue(
            Dictionary<string, string> fields,
            string fieldName)
        {
            return fields.TryGetValue(fieldName, out var value)
                ? value
                : "";
        }

        private static decimal GetDecimalField(
            Dictionary<string, string> fields,
            string fieldName)
        {
            if (!fields.TryGetValue(fieldName, out var value))
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            value = value.Trim().Replace(",", ".");

            return decimal.TryParse(
                value,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var result)
                ? result
                : 0;
        }
    }
}
