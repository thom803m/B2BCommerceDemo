using B2BCommerceDemo.Core.DTOs.Import;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IPurchaseOrderImportService
    {
        Task<ImportResult> ImportCsvAsync(Stream fileStream);
    }
}

