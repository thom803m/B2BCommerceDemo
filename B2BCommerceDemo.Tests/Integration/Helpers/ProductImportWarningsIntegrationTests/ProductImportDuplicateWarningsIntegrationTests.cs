using B2BCommerceDemo.Core.DTOs.Import;
using B2BCommerceDemo.Infrastructure.Imports.Helpers;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Helpers.ProductImportWarningsIntegration
{
    public class ProductImportDuplicateWarningsIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task HasDuplicateEanWarning_Should_Return_False_When_Ean_Is_Null()
        {
            var result = new ImportResult();

            var duplicate =
                await ProductImportWarnings.HasDuplicateEanWarning(
                    Context,
                    result,
                    [],
                    null,
                    "SKU001",
                    null);

            duplicate.Should().BeFalse();
            result.Warnings.Should().BeEmpty();
        }

        [Fact]
        public async Task HasDuplicateEanWarning_Should_Detect_Duplicate_In_File()
        {
            var result = new ImportResult();

            Dictionary<string, List<string>> seenEans =
                new()
                {
                    ["1234567890123"] = ["SKU001"]
                };

            var duplicate =
                await ProductImportWarnings.HasDuplicateEanWarning(
                    Context,
                    result,
                    seenEans,
                    null,
                    "SKU002",
                    "1234567890123");

            duplicate.Should().BeTrue();

            result.Warnings.Should().ContainSingle();

            result.Warnings[0]
                .Should()
                .Contain("Duplicate EAN 1234567890123 found in import file");
        }

        [Fact]
        public async Task HasDuplicateEanWarning_Should_Detect_Duplicate_In_Database()
        {
            var product = await CreateProductAsync();

            product.Ean = "1234567890123";

            await Context.SaveChangesAsync();

            var result = new ImportResult();

            var duplicate =
                await ProductImportWarnings.HasDuplicateEanWarning(
                    Context,
                    result,
                    [],
                    null,
                    "SKU002",
                    "1234567890123");

            duplicate.Should().BeTrue();

            result.Warnings.Should().ContainSingle();

            result.Warnings[0]
                .Should()
                .Contain("Duplicate EAN 1234567890123 already exists");
        }

        [Fact]
        public async Task HasDuplicateEanWarning_Should_Return_False_For_Unique_Ean()
        {
            var result = new ImportResult();

            Dictionary<string, List<string>> seenEans = [];

            var duplicate =
                await ProductImportWarnings.HasDuplicateEanWarning(
                    Context,
                    result,
                    seenEans,
                    null,
                    "SKU001",
                    "1234567890123");

            duplicate.Should().BeFalse();

            seenEans.Should().ContainKey("1234567890123");

            result.Warnings.Should().BeEmpty();
        }
    }
}

