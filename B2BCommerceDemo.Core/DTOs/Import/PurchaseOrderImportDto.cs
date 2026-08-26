namespace B2BCommerceDemo.Core.DTOs.Import
{
    public class PurchaseOrderImportDto
    {
        public string Sku { get; set; } = "";
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? InvoicedQuantity { get; set; }
    }
}

