using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/companyprices")]
    [Authorize(Roles = "Admin")]
    public class CompanyPricesController : ControllerBase
    {
        private readonly ICompanyPriceService _service;

        public CompanyPricesController(ICompanyPriceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companyPrices = await _service.GetAllAsync();

            return Ok(companyPrices);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyPriceDto dto)
        {
            var companyPrices = await _service.CreateAsync(dto);

            return Ok(companyPrices);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCompanyPriceDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}
