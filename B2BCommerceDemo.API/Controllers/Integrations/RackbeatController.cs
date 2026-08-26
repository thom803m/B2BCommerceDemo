using B2BCommerceDemo.Core.Interfaces.Integrations.Rackbeat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers.Integrations
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RackbeatController : ControllerBase
    {
        private readonly IRackbeatProductSyncService _productSyncService;
        private readonly IRackbeatPurchaseOrderSyncService _purchaseOrderSyncService;
        private readonly IRackbeatOrderStatusSyncService _orderStatusSyncService;

        public RackbeatController(
            IRackbeatProductSyncService productSyncService, 
            IRackbeatPurchaseOrderSyncService purchaseOrderSyncService,
            IRackbeatOrderStatusSyncService orderStatusSyncService)
        {
            _productSyncService = productSyncService;
            _purchaseOrderSyncService = purchaseOrderSyncService;
            _orderStatusSyncService = orderStatusSyncService;
        }

        [HttpPost("sync-products")]
        public async Task<IActionResult> SyncProducts(CancellationToken cancellationToken)
        {
            var result = await _productSyncService.SyncProductsAsync(cancellationToken);

            return Ok(result);
        }

        [HttpPost("sync-expected-deliveries")]
        public async Task<IActionResult> SyncExpectedDeliveries(CancellationToken cancellationToken)
        {
            var result = await _purchaseOrderSyncService.SyncExpectedDeliveriesAsync(cancellationToken);

            return Ok(result);
        }

        [HttpPost("sync-order-statuses")]
        public async Task<IActionResult> SyncOrderStatuses(CancellationToken cancellationToken)
        {
            var result = await _orderStatusSyncService.SyncOrderStatusesAsync(cancellationToken);

            return Ok(result);
        }
    }
}

