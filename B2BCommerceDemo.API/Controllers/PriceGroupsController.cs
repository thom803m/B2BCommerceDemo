using B2BCommerceDemo.Core.DTOs.PriceGroups;
using B2BCommerceDemo.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PriceGroupsController : ControllerBase
    {
        private readonly IPriceGroupService _priceGroupService;

        public PriceGroupsController(IPriceGroupService priceGroupService)
        {
            _priceGroupService = priceGroupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _priceGroupService.GetAllAsync();

            return Ok(groups);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePriceGroupDto dto)
        {
            var result = await _priceGroupService.UpdateAsync(id, dto);

            return Ok(result);
        }
    }
}
