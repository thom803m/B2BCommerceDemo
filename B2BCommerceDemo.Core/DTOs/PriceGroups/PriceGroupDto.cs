namespace B2BCommerceDemo.Core.DTOs.PriceGroups
{
    public class PriceGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal PercentageAdjustment { get; set; }
    }
}
