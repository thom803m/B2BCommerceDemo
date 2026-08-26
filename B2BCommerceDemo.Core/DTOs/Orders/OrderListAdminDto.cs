namespace B2BCommerceDemo.Core.DTOs.Orders
{
    public class OrderListAdminDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? Status { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

