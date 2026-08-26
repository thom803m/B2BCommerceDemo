namespace B2BCommerceDemo.Core.DTOs.Orders
{
    public class CreateOrderResult
    {
        public OrderDto Order { get; set; } = null!;
        public bool WasCreated { get; set; }
    }
}

