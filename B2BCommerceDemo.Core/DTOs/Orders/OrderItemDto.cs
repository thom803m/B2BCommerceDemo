namespace B2BCommerceDemo.Core.DTOs.Orders
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string? Sku { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}

