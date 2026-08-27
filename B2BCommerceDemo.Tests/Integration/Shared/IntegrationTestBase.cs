using B2BCommerceDemo.Core.DTOs.Integrations.Icecat;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Integrations.Icecat;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Events;
using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using B2BCommerceDemo.Infrastructure.Integrations.Icecat;
using B2BCommerceDemo.Infrastructure.Integrations.Rackbeat;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Infrastructure.Services.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace B2BCommerceDemo.Tests.Integration.Shared
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly AppDbContext Context;

        protected readonly IServiceProvider Services;

        protected readonly UserManager<ApplicationUser> UserManager;

        protected readonly RoleManager<IdentityRole> RoleManager;

        protected TestEventDispatcher EventDispatcher { get; private set; } = null!;

        private readonly SqliteConnection _connection;

        protected IntegrationTestBase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var services = new ServiceCollection();

            ConfigureServices(services);

            EventDispatcher = new TestEventDispatcher();

            services.AddSingleton<IEventDispatcher>(EventDispatcher);

            Services = services.BuildServiceProvider();

            Context = Services.GetRequiredService<AppDbContext>();

            Context.Database.EnsureCreated();

            UserManager = Services.GetRequiredService<UserManager<ApplicationUser>>();

            RoleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();

            EnsureRolesCreated();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "super-secret-test-key-super-secret-test-key" },
                { "Jwt:Issuer", "test-issuer" },
                { "Jwt:Audience", "test-audience" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IEventDispatcher, EventDispatcher>();

            services.AddScoped<IValidateUniqueness, ValidateUniqueness>();

            services.AddScoped<ICompanyAccessValidator, CompanyAccessValidator>();

            // Icecat Integration
            services.Configure<IcecatOptions>(options =>
            {
                options.Enabled = true;
            });

            IcecatClientMock = new Mock<IIcecatClient>();

            IcecatClientMock
                .Setup(x => x.GetProductByBrandAndSkuAsync(
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .ReturnsAsync((IcecatProductResponse?)null);

            services.AddSingleton(IcecatClientMock.Object);

            // Rackbeat Integration
            RackbeatClientMock = new Mock<IRackbeatClient>();

            services.AddSingleton(RackbeatClientMock.Object);

            services.AddScoped<IRackbeatCustomerSyncService, RackbeatCustomerSyncService>();
            services.AddScoped<IRackbeatOrderSyncService, RackbeatOrderSyncService>();
            services.AddScoped<IRackbeatOrderStatusSyncService, RackbeatOrderStatusSyncService>();
            services.AddScoped<IRackbeatProductSyncService, RackbeatProductSyncService>();
            services.AddScoped<IRackbeatPurchaseOrderSyncService, RackbeatPurchaseOrderSyncService>();

            // Add other necessary services and dependencies here
            services.AddScoped<IProductContentEnrichmentService, ProductContentEnrichmentService>();

            services.AddScoped<IClock, SystemClock>();

            services.AddScoped<IJwtService, JwtService>();

            services.AddScoped<IPriceService, PriceService>();

            services.AddScoped<IProductImageService, ProductImageService>();

            services.AddScoped<ProductService>();

            services.AddScoped<BrandService>();

            services.AddScoped<CategoryService>();

            services.AddScoped<CompanyService>();

            services.AddScoped<AuthService>();

            services.AddScoped<CartService>();

            services.AddScoped<OrderService>();

            services.AddScoped<PriceService>();

            services.AddScoped<ProductImageService>();

            services.AddScoped<ProductExportService>();

            services.AddScoped<ProductImportService>();

            services.AddScoped<IProductImportService>(
                serviceProvider => serviceProvider.GetRequiredService<ProductImportService>());

            services.AddScoped<PurchaseOrderImportService>();

            services.AddScoped<ProductImportCleanup>();

            services.AddScoped<ProductImportImageHandler>();
        }

        public void Dispose()
        {
            Context.Dispose();

            (_connection as IDisposable)?.Dispose();

            if (Services is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        protected T GetService<T>()
            where T : notnull
        {
            return Services.GetRequiredService<T>();
        }

        protected void ResetContext()
        {
            Context.ChangeTracker.Clear();
        }

        private void EnsureRolesCreated()
        {
            if (!RoleManager.RoleExistsAsync("User").Result)
            {
                RoleManager.CreateAsync(
                    new IdentityRole("User")).Wait();
            }

            if (!RoleManager.RoleExistsAsync("Admin").Result)
            {
                RoleManager.CreateAsync(
                    new IdentityRole("Admin")).Wait();
            }
        }

        protected async Task<PriceGroup> CreatePriceGroupAsync(
            string name = "Default")
        {
            var priceGroup = new PriceGroup
            {
                Name = name
            };

            Context.PriceGroups.Add(priceGroup);

            await Context.SaveChangesAsync();

            return priceGroup;
        }

        protected async Task<Product> CreateProductAsync(
            string name = "Test Product",
            string sku = "",
            string ean = "",
            decimal basePrice = 100m,
            int stock = 100,
            bool isActive = true,
            int? brandId = null,
            int? categoryId = null)
        {
            var product = new Product
            {
                Sku = string.IsNullOrWhiteSpace(sku)
                    ? Guid.NewGuid().ToString()
                    : sku,
                Ean = string.IsNullOrWhiteSpace(ean)
                    ? Guid.NewGuid().ToString("N")[..13]
                    : ean,
                Name = name,
                BasePrice = basePrice,
                AvailableStock = stock,
                IsActive = isActive,
                BrandId = brandId,
                CategoryId = categoryId,
                LastSynced = DateTime.UtcNow
            };

            Context.Products.Add(product);

            await Context.SaveChangesAsync();

            return product;
        }

        protected async Task<Brand> CreateBrandAsync(
            string name = "Apple")
        {
            var brand = new Brand
            {
                Name = name
            };

            Context.Brands.Add(brand);

            await Context.SaveChangesAsync();

            return brand;
        }

        protected async Task<Category> CreateCategoryAsync(
            string name = "Phones")
        {
            var category = new Category
            {
                Name = name
            };

            Context.Categories.Add(category);

            await Context.SaveChangesAsync();

            return category;
        }

        protected async Task<Company> CreateCompanyAsync()
        {
            var company = new Company
            {
                Name = "Test Company",
                Status = CompanyStatus.Active,
                PriceGroup = new PriceGroup
                {
                    PercentageAdjustment = 10m
                }
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();

            return company;
        }

        protected async Task<ApplicationUser> CreateUserAsync(
            string email = "test@test.dk",
            string password = "Test123!",
            bool emailConfirmed = true,
            int? companyId = null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = emailConfirmed,
                CompanyId = companyId
            };

            var result = await UserManager.CreateAsync(user, password);

            result.Succeeded.Should().BeTrue();

            return user;
        }

        protected async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);

            return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
                System.Text.Encoding.UTF8.GetBytes(token));
        }

        protected async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);

            return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
                System.Text.Encoding.UTF8.GetBytes(token));
        }

        public sealed class TestEventDispatcher : IEventDispatcher
        {
            private readonly List<object> _events = [];

            public IReadOnlyCollection<object> Events => _events;

            public Task PublishAsync<TEvent>(TEvent @event)
            {
                _events.Add(@event!);
                return Task.CompletedTask;
            }

            public List<TEvent> GetEvents<TEvent>()
            {
                return _events
                    .OfType<TEvent>()
                    .ToList();
            }

            public void Clear()
            {
                _events.Clear();
            }
        }

        protected Mock<IIcecatClient> IcecatClientMock { get; private set; } = null!;

        protected Mock<IRackbeatClient> RackbeatClientMock { get; private set; } = null!;
    }
}
