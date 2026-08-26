using System.Text.Json.Serialization;

namespace B2BCommerceDemo.Core.DTOs.Integrations.Rackbeat
{
    public class RackbeatOrderResponse
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("is_booked")]
        public bool IsBooked { get; set; }

        [JsonPropertyName("is_cancelled")]
        public bool IsCancelled { get; set; }

        [JsonPropertyName("is_shipped")]
        public bool IsShipped { get; set; }

        [JsonPropertyName("is_invoiced")]
        public bool IsInvoiced { get; set; }

        [JsonPropertyName("is_ready_for_shipping")]
        public bool IsReadyForShipping { get; set; }
    }
}
