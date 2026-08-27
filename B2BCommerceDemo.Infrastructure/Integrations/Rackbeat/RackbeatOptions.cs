namespace B2BCommerceDemo.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatOptions
    {
        public bool Enabled { get; set; }
        public string BaseUrl { get; set; } = "";
        public string ApiKey { get; set; } = "";
    }
}

