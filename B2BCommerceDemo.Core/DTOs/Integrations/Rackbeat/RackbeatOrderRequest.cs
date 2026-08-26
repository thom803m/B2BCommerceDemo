using System.Text.Json.Serialization;

namespace B2BCommerceDemo.Core.DTOs.Integrations.Rackbeat
{
    public class RackbeatOrderRequest
    {
        [JsonPropertyName("customer_id")]
        public string CustomerNumber { get; set; } = "";

        [JsonPropertyName("lines")]
        public List<RackbeatOrderLineRequest> Lines { get; set; } = new();
    }

    public class RackbeatOrderLineRequest
    {
        [JsonPropertyName("item_id")]
        public string ItemNumber { get; set; } = "";

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }
    }
}
