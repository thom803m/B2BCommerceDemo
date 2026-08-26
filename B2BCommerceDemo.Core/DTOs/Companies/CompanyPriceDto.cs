namespace B2BCommerceDemo.Core.DTOs.Companies
{
    public class CompanyPriceDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateCompanyPriceDto
    {
        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateCompanyPriceDto
    {
        public decimal Price { get; set; }
    }
}

