using System.ComponentModel.DataAnnotations;

namespace B2BCommerceDemo.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public decimal BasePrice { get; set; }
        public string? Ean { get; set; }
        public int AvailableStock { get; set; }
        public int PurchasedQuantity { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public bool IsActive { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
        public DateTime LastSynced { get; set; }
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<CompanyPrice> CompanyPrices { get; set; } = new List<CompanyPrice>();

        // For Icecat
        public string? IcecatName { get; set; }
        public string? Description { get; set; }
        public string? SpecificationsJson { get; set; }
        public string? IcecatProductId { get; set; }
        public DateTime? IcecatLastSynced { get; set; }
        public string? ContentSource { get; set; } // Rackbeat, Manual, Icecat
        public bool ContentLocked { get; set; }
    }
}

