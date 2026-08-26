namespace B2BCommerceDemo.Core.Events.Orders
{
    public class OrderShippedEvent
    {
        public int OrderId { get; set; }
        public int CompanyId { get; set; }
        public string? UserEmail { get; set; }
    }
}

