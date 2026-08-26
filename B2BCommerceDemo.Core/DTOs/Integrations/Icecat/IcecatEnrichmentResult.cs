namespace B2BCommerceDemo.Core.DTOs.Integrations.Icecat
{
    public class IcecatEnrichmentResult
    {
        public int Checked { get; set; }
        public int FullyEnriched { get; set; }
        public int PartiallyEnriched { get; set; }
        public int FullIcecatRequired { get; set; }
        public int NotFound { get; set; }
        public int Failed { get; set; }
        public List<string> Warnings { get; set; } = new();
    }
}
