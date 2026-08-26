namespace B2BCommerceDemo.Core.DTOs.Products
{
    public class ProductQueryParameters
    {
        public string? Search { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? Sku { get; set; }
        public string? Ean { get; set; }
        public bool? InStock { get; set; }
        public bool? IsPurchased { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? ContentSource { get; set; }
        public bool? ContentLocked { get; set; }
        public bool? HasIcecatProductId { get; set; }
        public bool? HasContent { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

