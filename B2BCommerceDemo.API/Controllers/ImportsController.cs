using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/import")]
    [Authorize(Roles = "Admin")]
    public class ImportsController : ControllerBase
    {
        private readonly IProductImportService _importService;
        private readonly IPurchaseOrderImportService _purchaseOrderImportService;

        public ImportsController(IProductImportService importService, IPurchaseOrderImportService purchaseOrderImportService)
        {
            _importService = importService;
            _purchaseOrderImportService = purchaseOrderImportService;
        }

        [HttpPost("products")]
        public async Task<IActionResult> ImportFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            var fileName = file.FileName.ToLowerInvariant();
            ImportResult result;

            if (fileName.EndsWith(".csv"))
            {
                result = await _importService.ImportCsvAsync(file.OpenReadStream());
            }
            else if (fileName.EndsWith(".xml"))
            {
                result = await _importService.ImportXmlAsync(file.OpenReadStream());
            }
            else
            {
                return BadRequest("Invalid file type. Only use CSV or XML.");
            }

            return Ok(result);
        }

        [HttpPost("delivery-dates")]
        public async Task<IActionResult> ImportPurchaseOrders(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            using var stream = file.OpenReadStream();

            var result = await _purchaseOrderImportService.ImportCsvAsync(stream);

            return Ok(result);
        }
    }
}
