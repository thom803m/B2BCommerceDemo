namespace B2BCommerceDemo.Core.Events.Orders
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public int CompanyId { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
    }
}

