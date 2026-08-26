using B2BCommerceDemo.Core.DTOs.Import;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IProductImportService
    {
        Task<ImportResult> ImportCsvAsync(Stream filestream);

        Task<ImportResult> ImportXmlAsync(Stream filestream);
        Task<ImportResult> ImportRecordsAsync(List<ProductImportDto> records);
    }
}
