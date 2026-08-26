namespace B2BCommerceDemo.Core.DTOs.Carts
{
    public class CartDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Total);
    }
}

