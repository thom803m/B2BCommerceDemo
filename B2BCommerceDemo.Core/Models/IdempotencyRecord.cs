namespace B2BCommerceDemo.Core.Models
{
    public class IdempotencyRecord
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public int CompanyId { get; set; }
        public int? OrderId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

