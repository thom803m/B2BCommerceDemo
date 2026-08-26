using B2BCommerceDemo.API.Controllers.Base;
using B2BCommerceDemo.Core.DTOs.Orders;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Users;
using B2BCommerceDemo.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService, IUserContext userContext)
            : base(userContext) 
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync(GetCompanyId(), GetUserId());

            return Ok(orders);
        }

        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(
                GetCompanyId(),
                GetUserId(),
                id);

            return Ok(order);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder( CreateOrderRequest request)
        {
            var order = await _orderService.CreateFromCartAsync(GetCompanyId(), GetUserId(), request.IdempotencyKey);

            if (order.WasCreated)
            {
                return CreatedAtAction(nameof(GetOrder), new { id = order.Order.Id }, order.Order);
            }

            return Ok(order.Order);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetOrdersAdmin([FromQuery] OrderQueryParameters parameters)
        {
            var result = await _orderService.GetOrdersAdminAsync(parameters);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetOrderAdmin(int id)
        {
            var order = await _orderService.GetOrderByIdAdminAsync(id);

            return Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus status)
        {
            var order = await _orderService.UpdateStatusAsync(id, status);

            return Ok(order);
        }
    }
}

