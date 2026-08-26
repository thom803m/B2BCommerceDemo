namespace B2BCommerceDemo.Core.DTOs.Import
{
    public class ImportResult
    {
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
        public List<string> Warnings { get; set; } = new();
    }
}

