namespace B2BCommerceDemo.Core.Models
{
    public class CompanyPrice
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        public decimal Price { get; set; }
    }
}

