namespace B2BCommerceDemo.Core.Events.Companies
{
    public class CompanyRejectedEvent
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = "";
        public string UserEmail { get; set; } = "";
    }
}
