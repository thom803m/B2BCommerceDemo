using System.ComponentModel.DataAnnotations;

namespace B2BCommerceDemo.Core.DTOs.Companies
{
    public class ApproveCompanyDto
    {
        [Range(1, int.MaxValue)]
        public int PriceGroupId { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Rackbeat customer number must contain only digits.")]
        public string RackbeatCustomerNumber { get; set; } = string.Empty;
    }
}

