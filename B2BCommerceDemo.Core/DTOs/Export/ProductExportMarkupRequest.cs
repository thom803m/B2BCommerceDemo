namespace B2BCommerceDemo.Core.DTOs.Export
{
    public class ProductExportMarkupRequest
    {
        public List<string> Fields { get; set; } = new();
        public decimal Percentage { get; set; }
    }
}
