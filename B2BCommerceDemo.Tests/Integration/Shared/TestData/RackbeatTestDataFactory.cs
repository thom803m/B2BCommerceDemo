using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.DTOs.Integrations.Rackbeat;

namespace B2BCommerceDemo.Tests.Integration.Shared.TestData
{
    public static class RackbeatTestDataFactory
    {
        public static ProductImportDto CreateProduct(
            string sku = "SKU-001",
            string name = "Rackbeat product",
            string ean = "1234567890123",
            decimal basePrice = 100m,
            int availableStock = 10,
            int purchasedQuantity = 0,
            string brand = "Zebra",
            string category = "Barcode Scanner",
            string? imageUrl = null,
            DateTime? expectedDeliveryDate = null)
        {
            return new ProductImportDto
            {
                Sku = sku,
                Name = name,
                Ean = ean,
                BasePrice = basePrice,
                AvailableStock = availableStock,
                PurchasedQuantity = purchasedQuantity,
                Brand = brand,
                Category = category,
                ImageUrl = imageUrl,
                ExpectedDeliveryDate = expectedDeliveryDate
            };
        }

        public static PurchaseOrderImportDto
            CreateExpectedDelivery(
                string sku = "SKU-001",
                DateTime? expectedDeliveryDate = null,
                decimal? quantity = 10m,
                decimal? receivedQuantity = 0m,
                decimal? invoicedQuantity = 0m)
        {
            return new PurchaseOrderImportDto
            {
                Sku = sku,
                ExpectedDeliveryDate =
                    expectedDeliveryDate,
                Quantity = quantity,
                ReceivedQuantity =
                    receivedQuantity,
                InvoicedQuantity =
                    invoicedQuantity
            };
        }

        public static RackbeatOrderResponse
            CreateOrderResponse(
                int number = 1001,
                bool isBooked = true,
                bool isCancelled = false,
                bool isShipped = false,
                bool isInvoiced = false,
                bool isReadyForShipping = false)
        {
            return new RackbeatOrderResponse
            {
                Number = number,
                IsBooked = isBooked,
                IsCancelled = isCancelled,
                IsShipped = isShipped,
                IsInvoiced = isInvoiced,
                IsReadyForShipping =
                    isReadyForShipping
            };
        }

        public static List<ProductImportDto>
            CreateProductList()
        {
            return
            [
                CreateProduct(
                    sku: "SKU-001",
                    name: "Rackbeat product 1",
                    ean: "1234567890101"),

                CreateProduct(
                    sku: "SKU-002",
                    name: "Rackbeat product 2",
                    ean: "1234567890102")
            ];
        }

        public static List<PurchaseOrderImportDto>
            CreateExpectedDeliveryList( DateTime? expectedDeliveryDate = null)
        {
            return
            [
                CreateExpectedDelivery(
                    sku: "SKU-001",
                    expectedDeliveryDate:
                        expectedDeliveryDate,
                    quantity: 10m),

                CreateExpectedDelivery(
                    sku: "SKU-002",
                    expectedDeliveryDate:
                        expectedDeliveryDate,
                    quantity: 5m)
            ];
        }
    }
}
