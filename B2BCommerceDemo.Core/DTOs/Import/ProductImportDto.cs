namespace B2BCommerceDemo.Core.DTOs.Import
{
    public class ProductImportDto
    {
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public int AvailableStock { get; set; }
        public int PurchasedQuantity { get; set; }
        public string Ean { get; set; } = "";
        public decimal BasePrice { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Brand { get; set; } = "";
        public string Category { get; set; } = "";
        public string? ImageUrl { get; set; }
    }
}

