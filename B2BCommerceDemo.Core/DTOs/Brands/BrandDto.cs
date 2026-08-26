using System.ComponentModel.DataAnnotations;

namespace B2BCommerceDemo.Core.DTOs.Brands
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class CreateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = "";
    }

    public class UpdateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = "";
    }
}

