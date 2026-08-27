namespace B2BCommerceDemo.Infrastructure.Integrations.Icecat
{
    public class IcecatOptions
    {
        public bool Enabled { get; set; }
        public string BaseUrl { get; set; } = "https://live.icecat.biz/api";
        public string Username { get; set; } = "";
        public string ApiToken { get; set; } = "";
        public string ContentToken { get; set; } = "";
        public string Language { get; set; } = "EN";
    }
}

