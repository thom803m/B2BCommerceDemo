namespace B2BCommerceDemo.Core.Models
{
    public class PriceGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal PercentageAdjustment { get; set; }
        public ICollection<Company> Companies { get; set; } = new List<Company>();
    }
}

