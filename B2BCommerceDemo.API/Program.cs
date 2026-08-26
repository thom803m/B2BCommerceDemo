using B2BCommerceDemo.API.Converters;
using B2BCommerceDemo.API.Middleware;
using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Events.Users;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Integrations.Icecat;
using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Interfaces.Users;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Data.Seed;
using B2BCommerceDemo.Infrastructure.EventHandlers.Users;
using B2BCommerceDemo.Infrastructure.Events;
using B2BCommerceDemo.Infrastructure.Events.Handlers.Companies;
using B2BCommerceDemo.Infrastructure.Events.Handlers.Orders;
using B2BCommerceDemo.Infrastructure.Events.Handlers.Orders.Rackbeat;
using B2BCommerceDemo.Infrastructure.Events.Handlers.Users;
using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using B2BCommerceDemo.Infrastructure.Integrations.Icecat;
using B2BCommerceDemo.Infrastructure.Integrations.Rackbeat;
using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Infrastructure.Services.Helpers;
using B2BCommerceDemo.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });

// Swagger / OpenAPI with a JWT Authorize button
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "B2B Commerce Demo API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter token"
    });

    // Security Requirement? Use Transformer?
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});


// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configure Rackbeat API Integrations
builder.Services.Configure<RackbeatOptions>(
    builder.Configuration.GetSection("Rackbeat"));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT signing key is not configured.");
}

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT signing key must contain at least 32 characters.");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role
    };
});

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductExportService, ProductExportService>();
builder.Services.AddScoped<IProductImportService, ProductImportService>();
builder.Services.AddScoped<IPurchaseOrderImportService, PurchaseOrderImportService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICompanyPriceService, CompanyPriceService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IClock, SystemClock>();
builder.Services.AddScoped<IPriceService, PriceService>();
builder.Services.AddScoped<IPriceGroupService, PriceGroupService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddScoped<ProductImportCleanup>();
builder.Services.AddScoped<ProductImportImageHandler>();

builder.Services.AddScoped<ICompanyAccessValidator, CompanyAccessValidator>();
builder.Services.AddScoped<IValidateUniqueness, ValidateUniqueness>();

builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IEventHandler<CompanyRegisteredEvent>, CompanyRegisteredEmailHandler>();
builder.Services.AddScoped<IEventHandler<CompanyApprovedEvent>, CompanyApprovedEmailHandler>();
builder.Services.AddScoped<IEventHandler<CompanyRejectedEvent>, CompanyRejectedEmailHandler>();
builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedAuditHandler>();
builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedEmailHandler>();
builder.Services.AddScoped<IEventHandler<OrderProcessingEvent>, OrderProcessingEmailHandler>();
builder.Services.AddScoped<IEventHandler<OrderShippedEvent>, OrderShippedEmailHandler>();
builder.Services.AddScoped<IEventHandler<OrderCompletedEvent>, OrderCompletedEmailHandler>();
builder.Services.AddScoped<IEventHandler<OrderCancelledEvent>, OrderCancelledEmailHandler>();
builder.Services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
builder.Services.AddScoped<IEventHandler<PasswordResetRequestedEvent>, PasswordResetRequestedEventHandler>();

// Rackbeat event handlers
var syncOrdersToRackbeat = builder.Configuration.GetValue<bool>("OrderProcessing:SyncToRackbeat");
if (syncOrdersToRackbeat)
{
    builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedRackbeatHandler>();
}

// Rackbeat API Client and Service
builder.Services.AddHttpClient<IRackbeatClient, RackbeatClient>();
builder.Services.AddScoped<IRackbeatProductSyncService, RackbeatProductSyncService>();
builder.Services.AddScoped<IRackbeatPurchaseOrderSyncService, RackbeatPurchaseOrderSyncService>();
builder.Services.AddScoped<IRackbeatCustomerSyncService, RackbeatCustomerSyncService>();
builder.Services.AddScoped<IRackbeatOrderSyncService, RackbeatOrderSyncService>();
builder.Services.AddScoped<IRackbeatOrderStatusSyncService, RackbeatOrderStatusSyncService>();
var rackbeatBackgroundSyncEnabled =
    builder.Configuration.GetValue(
        "Rackbeat:BackgroundSyncEnabled",
        true);

if (rackbeatBackgroundSyncEnabled)
{
    builder.Services.AddHostedService<RackbeatSyncBackgroundService>();
}

// Icecat API Client and Service
builder.Services.AddScoped<IProductContentEnrichmentService, ProductContentEnrichmentService>();
var icecatBackgroundSyncEnabled =
    builder.Configuration.GetValue(
        "Icecat:BackgroundSyncEnabled",
        true);

if (icecatBackgroundSyncEnabled)
{
    builder.Services.AddHostedService<IcecatSyncBackgroundService>();
}

builder.Services.Configure<IcecatOptions>(
    builder.Configuration.GetSection("Icecat")
);

builder.Services.AddHttpClient<IIcecatClient, IcecatClient>(client =>
{
    client.BaseAddress = new Uri("https://live.icecat.biz/");
});

// Configure CORS and HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:57897")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.Logger.LogWarning(
    "Rackbeat order synchronization is {Status}.",
    syncOrdersToRackbeat
        ? "ENABLED"
        : "DISABLED");

// Exception handling middleware
app.UseGlobalExceptionHandling();

// Seed roles and admin user (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "B2B Commerce Demo API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.MapControllers().RequireAuthorization();

app.Run();
