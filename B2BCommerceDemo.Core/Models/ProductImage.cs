namespace B2BCommerceDemo.Core.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public bool IsPrimary { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // Icecat specific properties
        public string Source { get; set; } = "Manual";
        public string? ExternalId { get; set; }
        public DateTime? LastSynced { get; set; }
    }
}

