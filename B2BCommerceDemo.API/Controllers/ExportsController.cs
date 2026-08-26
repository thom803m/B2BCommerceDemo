using B2BCommerceDemo.API.Controllers.Base;
using B2BCommerceDemo.Core.DTOs.Export;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/export")]
    [Authorize]
    public class ExportController : BaseController
    {
        private readonly IProductExportService _exportService;

        public ExportController(IProductExportService exportService, IUserContext userContext)
            : base(userContext)
        {
            _exportService = exportService;
        }

        [HttpGet("products/fields")]
        public IActionResult GetFields()
        {
            var fields = _exportService.GetAvailableFields()
                .Select(f => new ExportProductFieldDto
                {
                    Key = f.Key,
                    Label = f.Header
                });

            return Ok(fields);
        }

        [HttpPost("products")]
        public async Task<IActionResult> ExportProducts([FromBody] ProductExportRequest request)
        {
            if (request.Fields == null || !request.Fields.Any())
            {
                return BadRequest("At least one export field must be selected.");
            }

            int? companyId = IsAdmin
            ? null
            : GetCompanyId();

            var file = await _exportService.ExportProductsToCsvAsync(request.Fields, companyId);

            return File(file, "text/csv", "products.csv");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("products/markup")]
        public async Task<IActionResult> ExportProductsWithMarkup([FromBody] ProductExportMarkupRequest request)
        {
            if (request.Fields == null || !request.Fields.Any())
            {
                return BadRequest("At least one export field must be selected.");
            }

            var file = await _exportService
                .ExportProductsWithMarkupToCsvAsync(
                    request.Fields,
                    request.Percentage);

            return File(file, "text/csv", "products.csv");
        }
    }
}
