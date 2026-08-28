namespace B2BCommerceDemo.Infrastructure.Services.Helpers
{
    public class EmailOptions
    {
        public bool Enabled { get; set; }

        public string Host { get; set; } = "";

        public int Port { get; set; } = 587;

        public bool UseSsl { get; set; } = true;

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string FromName { get; set; } = "B2B Commerce Demo";

        public string FromEmail { get; set; } = "";
    }
}
