using B2BCommerceDemo.API.Controllers.Base;
using B2BCommerceDemo.Core.DTOs.Products;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductContentEnrichmentService _productContentEnrichmentService;

        public ProductsController(
            IProductService productService, 
            IProductContentEnrichmentService productContentEnrichmentService, 
            IUserContext userContext)
            : base(userContext) 
        {
            _productService = productService;
            _productContentEnrichmentService = productContentEnrichmentService;
        }

        // Ptoduct management endpoints
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters parameters)
        {
            var result = await _productService.GetProductsAsync(parameters, UserContext.CompanyId, UserContext.IsAdmin);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id, UserContext.CompanyId, UserContext.IsAdmin);

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            var updated = await _productService.UpdateProductAsync(id, dto);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);

            return NoContent();
        }

        // Icecat integration methods
        [HttpPut("{id}/content")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProductContent(int id, UpdateProductContentDto dto)
        {
            var product = await _productService.UpdateProductContentAsync(id, dto);

            return Ok(product);
        }

        [HttpPost("{id}/enrich")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> EnrichProduct(int id)
        {
            var product = await _productContentEnrichmentService.EnrichProductAsync(id);

            return Ok(product);
        }

        [HttpPost("enrich-missing-content")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrichMissingContent(CancellationToken cancellationToken)
        {
            var result = await _productContentEnrichmentService.EnrichMissingContentAsync(cancellationToken);

            return Ok(result);
        }
    }
}

