using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Core.DTOs.Categories;
using B2BCommerceDemo.Core.DTOs.Images;
using System.ComponentModel.DataAnnotations;

namespace B2BCommerceDemo.Core.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal BasePrice { get; set; }
        public string Ean { get; set; } = default!;
        public int AvailableStock { get; set; }
        public int PurchasedQuantity { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public bool IsActive { get; set; }

        public string? IcecatName { get; set; }
        public string? Description { get; set; }
        public string? SpecificationsJson { get; set; }
        public string? IcecatProductId { get; set; }
        public DateTime? IcecatLastSynced { get; set; }
        public string? ContentSource { get; set; }
        public bool ContentLocked { get; set; }

        public BrandDto? Brand { get; set; }
        public CategoryDto? Category { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "SKU must be between 1 and 100 characters")]
        public string Sku { get; set; } = default!;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "BasePrice is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "BasePrice must be greater than 0")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "EAN is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "EAN must be between 1 and 50 characters")]
        [RegularExpression(@"^\d+$", ErrorMessage = "EAN must contain only digits")]
        public string Ean { get; set; } = default!;

        public int AvailableStock { get; set; }

        public int BrandId { get; set; }

        public int CategoryId { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "SKU must be between 1 and 100 characters")]
        public string Sku { get; set; } = default!;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "BasePrice is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "BasePrice must be greater than 0")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "EAN is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "EAN must be between 1 and 50 characters")]
        [RegularExpression(@"^\d+$", ErrorMessage = "EAN must contain only digits")]
        public string Ean { get; set; } = default!;

        public int BrandId { get; set; }

        public int CategoryId { get; set; }
    }
}

