using B2BCommerceDemo.Core.DTOs.PriceGroups;
using System.ComponentModel.DataAnnotations;

namespace B2BCommerceDemo.Core.DTOs.Companies
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? RackbeatCustomerNumber { get; set; }
        public PriceGroupDto? PriceGroup { get; set; }
    }

    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;
    }

    public class UpdateCompanyDto
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;
    }
}

