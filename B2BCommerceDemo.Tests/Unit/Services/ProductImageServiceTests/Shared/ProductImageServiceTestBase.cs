using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace B2BCommerceDemo.Tests.Unit.Services.ProductImageServiceTests.Shared
{
    public abstract class ProductImageServiceTestBase
    {
        protected static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(
                InMemoryEventId.TransactionIgnoredWarning
            )
        )
                .Options;

            return new AppDbContext(options);
        }

        protected static Product CreateProduct(int id = 1)
        {
            return new Product
            {
                Id = id,
                Name = $"Product {id}",
                Sku = $"SKU{id}",
                BasePrice = 100,
                RowVersion = BitConverter.GetBytes(id)
            };
        }

        protected static ProductImage CreateImage(
            int id = 1,
            int productId = 1,
            string url = "url")
        {
            return new ProductImage
            {
                Id = id,
                ProductId = productId,
                Url = url,
                IsPrimary = false
            };
        }

        protected static ProductImageService CreateService(AppDbContext context)
        {
            return new ProductImageService(context);
        }
    }
}
