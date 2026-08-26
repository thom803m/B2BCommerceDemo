using B2BCommerceDemo.Core.DTOs.Common;
using B2BCommerceDemo.Core.DTOs.Orders;
using B2BCommerceDemo.Core.Events.Orders;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Core.Policies;
using B2BCommerceDemo.Infrastructure.Data;
using B2BCommerceDemo.Infrastructure.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICompanyAccessValidator _companyAccessValidator;
        private readonly IClock _clock;
        private readonly IPriceService _priceService;

        public OrderService(
            AppDbContext context, 
            UserManager<ApplicationUser> userManager, 
            IEventDispatcher eventDispatcher, 
            ICompanyAccessValidator companyAccessValidator, 
            IClock clock,
            IPriceService priceService) 
        {
            _context = context;
            _userManager = userManager;
            _eventDispatcher = eventDispatcher;
            _companyAccessValidator = companyAccessValidator;
            _clock = clock;
            _priceService = priceService;
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int companyId, string userId)
        {
            await _companyAccessValidator.ValidateCompanyActiveAsync(companyId);

            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Where(o => o.CompanyId == companyId && o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(OrderMapper.Map).ToList();
        }

        public async Task<OrderDto> GetOrderByIdAsync(int companyId, string userId, int orderId)
        {
            await _companyAccessValidator.ValidateCompanyActiveAsync(companyId);

            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.CompanyId == companyId && 
                    o.UserId == userId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            return OrderMapper.Map(order);
        }

        public async Task<CreateOrderResult> CreateFromCartAsync(int companyId, string userId, string idempotencyKey)
        {
            await _companyAccessValidator.ValidateCompanyActiveAsync(companyId);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = await _context.IdempotencyRecords
                .FirstOrDefaultAsync(x =>
                    x.Key == idempotencyKey &&
                    x.CompanyId == companyId &&
                    x.UserId == userId);

                if (existing != null)
                {
                    var existingOrder = await _context.Orders
                        .Include(o => o.Items)
                        .FirstOrDefaultAsync(o => o.Id == existing.OrderId);

                    if (existingOrder == null)
                    {
                        throw new InvalidOperationException("Idempotency record exists but order missing");
                    }

                    return new CreateOrderResult
                    {
                        Order = OrderMapper.Map(existingOrder),
                        WasCreated = false
                    };
                }

                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => 
                        c.CompanyId == companyId &&
                        c.UserId == userId);

                if (cart == null || !cart.Items.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }

                var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                var prices = await _priceService.GetPricesForProductsAsync(productIds, companyId);

                var order = new Order
                {
                    CompanyId = companyId,
                    UserId = userId,
                    CreatedAt = _clock.UtcNow,
                    Items = new List<OrderItem>()
                };

                foreach (var item in cart.Items)
                {
                    if (!products.TryGetValue(item.ProductId, out var product))
                    {
                        throw new KeyNotFoundException($"Product {item.ProductId} not found");
                    }

                    if (!prices.TryGetValue(item.ProductId, out var price))
                    {
                        throw new KeyNotFoundException($"Price not found for product {item.ProductId}");
                    }

                    if (product.AvailableStock < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Not enough stock for {product.Name}. Available: {product.AvailableStock}");
                    }

                    product.AvailableStock -= item.Quantity;

                    order.Items.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Sku = product.Sku,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = price
                    });
                }

                order.Total = order.Items.Sum(x => x.Quantity * x.UnitPrice);

                var user = await _userManager.FindByIdAsync(userId);

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cart.Items);

                await _context.SaveChangesAsync();

                var idempotencyRecord = new IdempotencyRecord
                {
                    Key = idempotencyKey,
                    CompanyId = companyId,
                    UserId = userId,
                    OrderId = order.Id,
                    CreatedAt = _clock.UtcNow
                };

                _context.IdempotencyRecords.Add(idempotencyRecord);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _eventDispatcher.PublishAsync(new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    CompanyId = order.CompanyId,
                    UserId = order.UserId!,
                    UserEmail = user?.Email,
                    CreatedAt = order.CreatedAt,
                    Total = order.Total
                });

                return new CreateOrderResult
                {
                    Order = OrderMapper.Map(order),
                    WasCreated = true
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Stock changed during checkout. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PagedResult<OrderListAdminDto>> GetOrdersAdminAsync(OrderQueryParameters parameters)
        {
            var query = _context.Orders
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                if (Enum.TryParse<OrderStatus>(parameters.Status, true, out var status))
                {
                    query = query.Where(o => o.Status == status);
                }
            }

            if (parameters.CompanyId.HasValue)
            {
                query = query.Where(o => o.CompanyId == parameters.CompanyId.Value);
            }

            if (parameters.FromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= parameters.FromDate.Value);
            }

            if (parameters.ToDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= parameters.ToDate.Value);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(o => new OrderListAdminDto
                {
                    Id = o.Id,
                    CompanyId = o.CompanyId,
                    Status = o.Status.ToString(),
                    Total = o.Total,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<OrderListAdminDto>
            {
                Items = orders,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        public async Task<OrderDto> GetOrderByIdAdminAsync(int orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            return OrderMapper.Map(order);
        }

        public async Task<OrderDto> UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot change status of cancelled order");
            }

            OrderStatusPolicy.Validate(order.Status, newStatus);

            var oldStatus = order.Status;
            order.Status = newStatus;

            await _context.SaveChangesAsync();

            await _eventDispatcher.PublishAsync(new OrderStatusChangedEvent
            {
                OrderId = order.Id,
                CompanyId = order.CompanyId,
                OldStatus = oldStatus.ToString(),
                NewStatus = newStatus.ToString()
            });

            var user = await _userManager.FindByIdAsync(order.UserId!);

            switch (newStatus)
            {
                case OrderStatus.Confirmed:
                    await _eventDispatcher.PublishAsync(new OrderConfirmedEvent
                    {
                        OrderId = order.Id,
                        CompanyId = order.CompanyId,
                        UserEmail = user?.Email
                    });
                    break;

                case OrderStatus.Processing:
                    await _eventDispatcher.PublishAsync(new OrderProcessingEvent
                    {
                        OrderId = order.Id,
                        CompanyId = order.CompanyId,
                        UserEmail = user?.Email
                    });
                    break;

                case OrderStatus.Shipped:
                    await _eventDispatcher.PublishAsync(new OrderShippedEvent
                    {
                        OrderId = order.Id,
                        CompanyId = order.CompanyId,
                        UserEmail = user?.Email
                    });
                    break;

                case OrderStatus.Completed:
                    await _eventDispatcher.PublishAsync(new OrderCompletedEvent
                    {
                        OrderId = order.Id,
                        CompanyId = order.CompanyId,
                        UserEmail = user?.Email
                    });
                    break;

                case OrderStatus.Cancelled:
                    await _eventDispatcher.PublishAsync(new OrderCancelledEvent
                    {
                        OrderId = order.Id,
                        CompanyId = order.CompanyId,
                        UserEmail = user?.Email
                    });
                    break;
            }

            return OrderMapper.Map(order);
        }
    }
}

