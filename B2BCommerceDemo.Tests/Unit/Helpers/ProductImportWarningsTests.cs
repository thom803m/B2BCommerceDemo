using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Unit.Helpers
{
    public class ProductImportWarningsTests
    {
        [Fact]
        public void HasRequiredFieldWarnings_Should_Return_False_When_All_Fields_Are_Present()
        {
            var result = new ImportResult();

            var hasWarnings =
                ProductImportWarnings.HasRequiredFieldWarnings(
                    result,
                    "SKU001",
                    "1234567890123",
                    "Logitech",
                    "Mouse");

            hasWarnings.Should().BeFalse();
            result.Warnings.Should().BeEmpty();
        }

        [Fact]
        public void HasRequiredFieldWarnings_Should_Add_Warning_When_Ean_Is_Missing()
        {
            var result = new ImportResult();

            var hasWarnings =
                ProductImportWarnings.HasRequiredFieldWarnings(
                    result,
                    "SKU001",
                    null,
                    "Logitech",
                    "Mouse");

            hasWarnings.Should().BeTrue();

            result.Warnings.Should().ContainSingle()
                .Which.Should().Contain("EAN");
        }

        [Fact]
        public void HasRequiredFieldWarnings_Should_Add_Warning_When_Multiple_Fields_Are_Missing()
        {
            var result = new ImportResult();

            var hasWarnings =
                ProductImportWarnings.HasRequiredFieldWarnings(
                    result,
                    "SKU001",
                    null,
                    null,
                    null);

            hasWarnings.Should().BeTrue();

            result.Warnings.Should().ContainSingle();

            result.Warnings[0]
                .Should()
                .Be("SKU SKU001: Missing required fields: EAN, Brand, Category");
        }

        [Fact]
        public void AddScientificWarnings_Should_Add_Warning_When_Warning_Exists()
        {
            var result = new ImportResult();

            ProductImportWarnings.AddScientificWarnings(
                result,
                "SKU001",
                "Scientific notation detected");

            result.Warnings.Should().ContainSingle();

            result.Warnings[0]
                .Should()
                .Be("SKU SKU001: Scientific notation detected");
        }

        [Fact]
        public void AddScientificWarnings_Should_Do_Nothing_When_Warning_Is_Null()
        {
            var result = new ImportResult();

            ProductImportWarnings.AddScientificWarnings(
                result,
                "SKU001",
                null);

            result.Warnings.Should().BeEmpty();
        }
    }
}

