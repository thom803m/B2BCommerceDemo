using B2BCommerceDemo.Infrastructure.Services;
using B2BCommerceDemo.Tests.Integration.Shared;
using FluentAssertions;

namespace B2BCommerceDemo.Tests.Integration.Services.ProductImportServiceIntegrationTests
{
    public class ImportProductsFromXMLIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task ImportXmlAsync_Should_Create_Product()
        {
            var service = GetService<ProductImportService>();

            var xml =
                """
                <Products>
                  <Product>
                    <Sku>SKU001</Sku>
                    <Name>Xml Product</Name>
                    <Available>10</Available>
                    <Purchased>0</Purchased>
                    <Ean>1234567890123</Ean>
                    <BasePrice>100</BasePrice>
                    <Brand>Logitech</Brand>
                    <Category>Mouse</Category>
                  </Product>
                </Products>
                """;

            using var stream =
                new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(xml));

            var result = await service.ImportXmlAsync(stream);

            result.Created.Should().Be(1);
        }
    }
}

