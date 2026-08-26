using System.ComponentModel.DataAnnotations;

namespace B2BCommerceDemo.Core.DTOs.Companies
{
    public class UpdateCompanyPriceGroupDto
    {
        [Range(1, int.MaxValue)]
        public int PriceGroupId { get; set; }
    }
}

