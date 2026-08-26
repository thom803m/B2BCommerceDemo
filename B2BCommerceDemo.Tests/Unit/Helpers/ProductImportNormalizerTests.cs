using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Helpers
{
    public class ProductImportNormalizerTests
    {
        [Fact]
        public void NormalizeSku_Should_Trim_And_Uppercase()
        {
            var sku = " xf4050gb ";

            var result = ProductImportNormalizer.NormalizeSku(sku);

            result.Should().Be("XF4050GB");
        }

        [Fact]
        public void NormalizeSku_Should_Remove_Extra_Whitespace()
        {
            var sku = "   XF4050GB   ";

            var result = ProductImportNormalizer.NormalizeSku(sku);

            result.Should().Be("XF4050GB");
        }

        [Fact]
        public void NormalizeSku_Should_Convert_Scientific_Notation()
        {
            var sku = "5.70057E+12";

            var result = ProductImportNormalizer.NormalizeSku(sku);

            result.Should().Be("5700570000000");
        }

        [Fact]
        public void NormalizeKey_Should_Trim_And_Uppercase()
        {
            var value = " xf4050gb ";

            var result = ProductImportNormalizer.NormalizeKey(value);

            result.Should().Be("XF4050GB");
        }

        [Fact]
        public void IsDamagedBox_Should_Return_True_When_Sku_Contains_DB()
        {
            var sku = "XF4050GB_DB";

            var result = ProductImportNormalizer.IsDamagedBox(sku);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsDamagedBox_Should_Return_False_When_Sku_Does_Not_Contain_DB()
        {
            var sku = "XF4050GB";

            var result = ProductImportNormalizer.IsDamagedBox(sku);

            result.Should().BeFalse();
        }

        [Fact]
        public void NormalizeScientific_Should_Return_Empty_For_Null()
        {
            var ean = (string?)null;

            var (normalized, warning) = ProductImportNormalizer.NormalizeScientific(ean);

            normalized.Should().BeEmpty();
            warning.Should().BeNull();
        }

        [Fact]
        public void NormalizeScientific_Should_Return_Empty_For_Whitespace()
        {
            var ean = "   ";

            var (normalized, warning) = ProductImportNormalizer.NormalizeScientific(ean);

            normalized.Should().BeEmpty();
            warning.Should().BeNull();
        }

        [Fact]
        public void NormalizeScientific_Should_Convert_Scientific_Notation()
        {
            var ean = "5.70057E+12";

            var (normalized, warning) = ProductImportNormalizer.NormalizeScientific(ean);

            normalized.Should().Be("5700570000000");
            warning.Should().NotBeNull();
        }

        [Fact]
        public void NormalizeScientific_Should_Return_Warning_For_Scientific_Notation()
        {
            var ean = "5.70057E+12";

            var (normalized, warning) =
                ProductImportNormalizer.NormalizeScientific(ean);

            normalized.Should().Be("5700570000000");
            warning.Should().Be("Scientific notation detected: 5.70057E+12 ? 5700570000000");
        }

        [Fact]
        public void NormalizeScientific_Should_Return_Value_When_Not_Scientific()
        {
            var ean = "5700570000000";

            var (normalized, warning) = ProductImportNormalizer.NormalizeScientific(ean);

            normalized.Should().Be("5700570000000");
            warning.Should().BeNull();
        }

        [Fact]
        public void NormalizeScientific_Should_Return_Parse_Error_Warning()
        {
            var ean = "570057ABCE+12";

            var (normalized, warning) = ProductImportNormalizer.NormalizeScientific(ean);

            normalized.Should().Be("570057ABCE+12");
            warning.Should().Be("Failed to parse scientific notation: 570057ABCE+12");
        }
    }
}
