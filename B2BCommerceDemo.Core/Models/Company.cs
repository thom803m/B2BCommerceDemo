namespace B2BCommerceDemo.Core.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? PriceGroupId { get; set; }
        public PriceGroup? PriceGroup { get; set; } = null!;
        public CompanyStatus Status { get; set; } = CompanyStatus.Pending;

        public string? RackbeatCustomerNumber { get; set; }

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<CompanyPrice> CompanyPrices { get; set; } = new List<CompanyPrice>();
    }

    public enum CompanyStatus
    {
        Pending = 0,
        Active = 1,
        Rejected = 2,
        Suspended = 3
    }
}

