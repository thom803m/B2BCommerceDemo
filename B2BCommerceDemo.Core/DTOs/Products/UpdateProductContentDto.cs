namespace B2BCommerceDemo.Core.DTOs.Products
{
    public class UpdateProductContentDto
    {
        public string? Description { get; set; }
        public string? SpecificationsJson { get; set; }
        public bool ContentLocked { get; set; }
    }
}
