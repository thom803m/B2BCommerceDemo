namespace B2BCommerceDemo.Core.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }
        public string? RackbeatOrderNumber { get; set; }
        public string? RackbeatSyncStatus { get; set; }
        public string? RackbeatSyncError { get; set; }
        public DateTime? RackbeatSyncedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}

