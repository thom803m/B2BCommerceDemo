using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllAsync(); 

            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _companyService.GetByIdAsync(id);

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyDto dto)
        {
            var company = await _companyService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCompanyDto dto)
        {
            var result = await _companyService.UpdateAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _companyService.SuspendAsync(id);

            return NoContent();
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCompanies()
        {
            var companies = await _companyService.GetPendingCompaniesAsync();

            return Ok(companies);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminCompanies()
        {
            var companies =
                await _companyService
                    .GetAdminCompaniesAsync();

            return Ok(companies);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id, ApproveCompanyDto dto)
        {
            await _companyService.ApproveCompanyAsync(id, dto);

            return NoContent();
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            await _companyService.RejectCompanyAsync(id);

            return NoContent();
        }

        [HttpPut("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(int id)
        {
            await _companyService
                .ReactivateAsync(id);

            return NoContent();
        }

        [HttpPut("{id}/pricegroup")]
        public async Task<IActionResult> UpdatePriceGroup(int id, UpdateCompanyPriceGroupDto dto)
        {
            await _companyService.UpdatePriceGroupAsync(id, dto.PriceGroupId);

            return NoContent();
        }
    }
}
