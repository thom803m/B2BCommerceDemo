namespace B2BCommerceDemo.Core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? UserId { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? RackbeatOrderNumber { get; set; }
        public RackbeatSyncStatus RackbeatSyncStatus { get; set; } = RackbeatSyncStatus.Pending;
        public string? RackbeatSyncError { get; set; }
        public DateTime? RackbeatSyncedAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
    
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Completed = 4,
        Cancelled = 5,
    }

    public enum RackbeatSyncStatus
    {
        Pending = 0,
        Synced = 1,
        Failed = 2
    }
}

