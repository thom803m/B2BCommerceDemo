using B2BCommerceDemo.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/images")]
    [Authorize(Roles = "Admin")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImagesController(IProductImageService service)
        {
            _service = service;
        }

        [HttpPost("url")]
        public async Task<IActionResult> UploadFromUrl(int productId, [FromBody] string imageUrl)
        {
            var image = await _service.UploadFromUrlAsync(productId, imageUrl);

            return Ok(image);
        }

        [HttpPost("{imageId}/primary")]
        public async Task<IActionResult> SetPrimary(int productId, int imageId)
        {
            await _service.SetPrimaryAsync(productId, imageId);

            return NoContent();
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int productId, int imageId)
        {
            await _service.DeleteImageAsync(productId, imageId);

            return NoContent();
        }
    }
}
