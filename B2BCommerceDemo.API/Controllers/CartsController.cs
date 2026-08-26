using B2BCommerceDemo.API.Controllers.Base;
using B2BCommerceDemo.Core.DTOs.Carts;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartsController : BaseController
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService, IUserContext userContext)
            : base(userContext)
        {
            _cartService = cartService;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync(GetCompanyId(), GetUserId());

            return Ok(cart);
        }

        [Authorize(Roles = "User")]
        [HttpPost("items")]
        public async Task<IActionResult> AddItem(CreateCartItemDto dto)
        {
            var cart = await _cartService.AddItemAsync(GetCompanyId(), GetUserId(), dto);

            return Ok(cart);
        }

        [Authorize(Roles = "User")]
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(int id, UpdateCartItemDto dto)
        {
            var cart = await _cartService.UpdateItemAsync(GetCompanyId(), GetUserId(), id, dto);

            return Ok(cart);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var cart = await _cartService.RemoveItemAsync(GetCompanyId(), GetUserId(), id);

            return Ok(cart);
        }
    }
}
