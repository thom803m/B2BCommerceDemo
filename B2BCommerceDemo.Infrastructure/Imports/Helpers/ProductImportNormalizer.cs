using System.Globalization;

namespace B2BCommerceDemo.Infrastructure.Imports.Helpers
{
    public static class ProductImportNormalizer
    {
        public static string NormalizeSku(string sku)
        {
            return NormalizeScientific(sku)
                .normalized
                .Trim()
                .ToUpperInvariant();
        }

        public static string NormalizeKey(string value)
        {
            return value
                .Trim()
                .ToUpperInvariant();
        }

        public static bool IsDamagedBox(string sku)
        {
            return sku.Contains("_DB");
        }

        public static (string normalized, string? warning)
            NormalizeScientific(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ("", null);
            }

            var original = value;

            value = value
                .Trim()
                .Replace(",", ".");

            if (value.Contains("E+") || value.Contains("e+"))
            {
                if (decimal.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var number))
                {
                    var normalized = decimal
                        .Truncate(number)
                        .ToString(CultureInfo.InvariantCulture);

                    return (
                        normalized,
                        $"Scientific notation detected: {original} ? {normalized}"
                    );
                }

                return (
                    value,
                    $"Failed to parse scientific notation: {original}"
                );
            }

            return (value, null);
        }
    }
}
