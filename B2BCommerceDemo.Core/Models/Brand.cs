using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Brand
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

