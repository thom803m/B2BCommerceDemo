namespace B2BCommerceDemo.Core.Interfaces.Services.Validate
{
    public interface IValidateUniqueness
    {
        Task ValidateUniqueSkuAsync(string sku, int? excludeProductId = null);
        Task ValidateUniqueEanAsync(string? ean, int? excludeProductId = null);
        Task ValidateUniqueBrandNameAsync(string name, int? excludeId = null);
        Task ValidateUniqueCategoryNameAsync(string name, int? excludeId = null);
        Task ValidateUniqueCompanyNameAsync(string name, int? excludeId = null);
    }
}

