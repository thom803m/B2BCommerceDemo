namespace B2BCommerceDemo.Core.Exports
{
    public class ExportFieldDefinition
    {
        public string Key { get; set; } = "";
        public string Header { get; set; } = "";
        public Func<Models.Product, string?> Selector { get; set; } = default!;
    }
}

