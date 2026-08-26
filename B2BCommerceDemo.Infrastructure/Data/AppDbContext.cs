using B2BCommerceDemo.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<CompanyPrice> CompanyPrices => Set<CompanyPrice>();

        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>(); 

        public DbSet<PriceGroup> PriceGroups { get; set; }

        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var provider = Database.ProviderName;

            var product = modelBuilder.Entity<Product>();

            if (provider == "Microsoft.EntityFrameworkCore.Sqlite"
                || provider == "Microsoft.EntityFrameworkCore.InMemory")
            {
                product.Property(p => p.RowVersion)
                    .ValueGeneratedNever()
                    .IsRequired(false);
            }
            else
            {
                product.Property(p => p.RowVersion)
                    .IsRowVersion();
            }

            // Product
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Ean)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.BasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_Product_Stock",
                        "[AvailableStock] >= 0"));

            // Image
            modelBuilder.Entity<ProductImage>()
                .HasIndex(x => new { x.ProductId, x.IsPrimary })
                    .HasFilter("[IsPrimary] = 1")
                    .IsUnique();

            // Brand
            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.Name)
                .IsUnique();

            // Category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Company
            modelBuilder.Entity<Company>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<IdempotencyRecord>()
                .HasIndex(x => new { x.Key, x.CompanyId, x.UserId })
                .IsUnique();

            modelBuilder.Entity<Company>()
                .HasOne(c => c.PriceGroup)
                .WithMany(pg => pg.Companies)
                .HasForeignKey(c => c.PriceGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .Property(c => c.Status)
                .HasConversion<string>();

            // CompanyPrice
            modelBuilder.Entity<CompanyPrice>()
                .Property(cp => cp.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompanyPrice>()
                .HasIndex(cp => new { cp.ProductId, cp.CompanyId })
                .IsUnique();

            // PriceGroup
            modelBuilder.Entity<PriceGroup>()
                .Property(x => x.PercentageAdjustment)
                .HasPrecision(18, 2);

            // Cart
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.CompanyId);

            modelBuilder.Entity<CartItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            // Order
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CompanyId);

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}
