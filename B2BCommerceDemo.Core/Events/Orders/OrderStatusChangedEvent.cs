namespace B2BCommerceDemo.Core.Events.Orders
{
    public class OrderStatusChangedEvent
    {
        public int OrderId { get; set; }
        public int CompanyId { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
    }
}

