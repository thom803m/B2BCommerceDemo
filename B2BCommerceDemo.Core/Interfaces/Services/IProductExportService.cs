using B2BCommerceDemo.Core.Exports;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IProductExportService
    {
        List<ExportFieldDefinition> GetAvailableFields();
        Task<byte[]> ExportProductsToCsvAsync(List<string>? selectedFields = null, int? companyId = null);
        Task<byte[]> ExportProductsWithMarkupToCsvAsync(List<string> selectedFields, decimal percentage);
    }
}

