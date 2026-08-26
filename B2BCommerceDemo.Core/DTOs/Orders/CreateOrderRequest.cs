namespace B2BCommerceDemo.Core.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public string IdempotencyKey { get; set; } = null!;
    }
}

