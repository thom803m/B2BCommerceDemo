using B2BCommerceDemo.Core.DTOs.Integrations.Icecat;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Tests.Integration.Shared;
using B2BCommerceDemo.Tests.Integration.Shared.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace B2BCommerceDemo.Tests.Integration.Integrations.Icecat.ProductContentEnrichment
{
    public class EnrichProductIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task EnrichProductAsync_Should_Return_Product_When_Product_Exists()
        {
            var service = GetService<IProductContentEnrichmentService>();

            await CreateProductAsync();

            var product = await Context.Products.SingleAsync();

            var result = await service.EnrichProductAsync(product.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(product.Id);
            result.Sku.Should().Be(product.Sku);
            result.Name.Should().Be(product.Name);
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Throw_When_Product_Does_Not_Exist()
        {
            var service = GetService<IProductContentEnrichmentService>();

            await FluentActions
                .Invoking(() => service.EnrichProductAsync(999999))
                .Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product not found");
        }

        [Fact]
        public async Task EnrichMissingContentAsync_Should_Skip_When_Icecat_Returns_No_Content()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Apple");

            var product = await CreateProductAsync(
                sku: "SKU-NO-CONTENT",
                ean: "1234567890123",
                brandId: brand.Id);

            IcecatClientMock
                .Setup(x => x.GetProductByBrandAndSkuAsync("Apple", "SKU-NO-CONTENT"))
                .ReturnsAsync(IcecatTestDataFactory.CreateEmptyResponse());

            var result = await service.EnrichMissingContentAsync();

            result.Checked.Should().Be(1);
            result.FullyEnriched.Should().Be(0);
            result.PartiallyEnriched.Should().Be(0);
            result.FullIcecatRequired.Should().Be(0);
            result.NotFound.Should().Be(1);
            result.Failed.Should().Be(0);
            result.Warnings.Should().ContainSingle();

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.ContentSource.Should().NotBe("Icecat");
            updated.Description.Should().BeNullOrWhiteSpace();
            updated.IcecatProductId.Should().BeNull();
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Save_Icecat_Images_With_Source_Icecat()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Apple");

            var product = await CreateProductAsync(
                sku: "SKU-123",
                ean: "1234567890123",
                brandId: brand.Id);

            IcecatClientMock
                .Setup(x => x.GetProductByBrandAndSkuAsync(
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .ReturnsAsync(IcecatTestDataFactory.CreateResponseWithImages());

            await service.EnrichProductAsync(product.Id);

            ResetContext();

            var images = await Context.ProductImages.Where(x => x.ProductId == product.Id).ToListAsync();

            images.Should().HaveCount(2);
            images.Should().OnlyContain(x => x.Source == "Icecat");
            images.Should().ContainSingle(x => x.IsPrimary);
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Remove_Rackbeat_Images_When_Icecat_Images_Exist()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Apple");

            var product = await CreateProductAsync(
                sku: "SKU-123",
                ean: "1234567890123",
                brandId: brand.Id);

            Context.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                Url = "https://cdn.rackbeat.com/test.jpg",
                IsPrimary = true,
                Source = "Rackbeat"
            });

            await Context.SaveChangesAsync();

            IcecatClientMock
                .Setup(x => x.GetProductByBrandAndSkuAsync(
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .ReturnsAsync(IcecatTestDataFactory.CreateResponseWithImages());

            await service.EnrichProductAsync(product.Id);

            ResetContext();

            var images = await Context.ProductImages.Where(x => x.ProductId == product.Id).ToListAsync();

            images.Should().HaveCount(2);
            images.Should().OnlyContain(x => x.Source == "Icecat");
            images.Should().NotContain(x => x.Source == "Rackbeat");
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Keep_Manual_Images_When_Icecat_Images_Exist()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Apple");

            var product = await CreateProductAsync(
                sku: "SKU-123",
                ean: "1234567890123",
                brandId: brand.Id);

            Context.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                Url = "https://manual.dk/manual.jpg",
                IsPrimary = true,
                Source = "Manual"
            });

            await Context.SaveChangesAsync();

            IcecatClientMock
                .Setup(x => x.GetProductByBrandAndSkuAsync(
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .ReturnsAsync(IcecatTestDataFactory.CreateResponseWithImages());

            await service.EnrichProductAsync(product.Id);

            ResetContext();

            var images = await Context.ProductImages.Where(x => x.ProductId == product.Id).ToListAsync();

            images.Should().HaveCount(3);
            images.Should().Contain(x => x.Source == "Manual");
            images.Should().Contain(x => x.Source == "Icecat");
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Not_Update_When_ContentLocked()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Apple");

            var product = await CreateProductAsync(
                sku: "SKU-123",
                ean: "1234567890123",
                brandId: brand.Id);

            product.IcecatName = "Existing manual Icecat name";
            product.Description = "Manual description";
            product.ContentSource = "Manual";
            product.ContentLocked = true;

            await Context.SaveChangesAsync();

            IcecatClientMock
                .Setup(x => x.GetProductByBrandAndSkuAsync(
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .ReturnsAsync(IcecatTestDataFactory.CreateResponseWithImages());

            await service.EnrichProductAsync(product.Id);

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.IcecatName.Should().Be("Existing manual Icecat name");
            updated.Description.Should().Be("Manual description");
            updated.ContentSource.Should().Be("Manual");
            updated.ContentLocked.Should().BeTrue();

            IcecatClientMock.Verify(
                x => x.GetProductByBrandAndSkuAsync(
                    It.IsAny<string?>(),
                    It.IsAny<string?>()),
                Times.Never);
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Save_GeneralInfo_Title_As_IcecatName()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Zebra");

            var product = await CreateProductAsync(
                name: "Rackbeat product name",
                sku: "20-71043-04R",
                ean: "5712505739549",
                brandId: brand.Id);

            IcecatClientMock
                .Setup(x =>
                    x.GetProductByBrandAndSkuAsync(
                        "Zebra",
                        "20-71043-04R"))
                .ReturnsAsync(
                    IcecatTestDataFactory.CreateCompleteResponse(
                        title: "Zebra 20-71043-04R holder Passive holder Barcode scanner Black",
                        productCode: "20-71043-04R"));

            var result = await service.EnrichProductAsync(product.Id);

            result.Should().NotBeNull();

            result!.Name.Should().Be("Zebra 20-71043-04R holder Passive holder Barcode scanner Black");
            result.IcecatName.Should().Be("Zebra 20-71043-04R holder Passive holder Barcode scanner Black");

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.Name.Should().Be("Rackbeat product name");
            updated.IcecatName.Should().Be("Zebra 20-71043-04R holder Passive holder Barcode scanner Black");
            updated.ContentSource.Should().Be("Icecat");
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Use_GeneratedLocalTitle_When_Title_Is_Missing()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Zebra");

            var product = await CreateProductAsync(
                name: "Rackbeat product name",
                sku: "SKU-LOCAL-TITLE",
                brandId: brand.Id);

            IcecatClientMock
                .Setup(x =>
                    x.GetProductByBrandAndSkuAsync(
                        "Zebra",
                        "SKU-LOCAL-TITLE"))
                .ReturnsAsync(
                    IcecatTestDataFactory
                        .CreateResponseWithGeneratedLocalTitle(
                            generatedTitle: "Generated local product title",
                            productCode: "SKU-LOCAL-TITLE"));

            var result = await service.EnrichProductAsync(product.Id);

            result.Should().NotBeNull();

            result!.Name.Should().Be( "Generated local product title");
            result.IcecatName.Should().Be( "Generated local product title");

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.Name.Should().Be( "Rackbeat product name");
            updated.IcecatName.Should().Be( "Generated local product title");
        }

        [Fact]
        public async Task EnrichProductAsync_Should_Keep_Existing_IcecatName_When_Response_Has_No_Title()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Zebra");

            var product = await CreateProductAsync(
                name: "Rackbeat product name",
                sku: "SKU-WITHOUT-TITLE",
                brandId: brand.Id);

            product.IcecatName = "Existing Icecat title";

            await Context.SaveChangesAsync();

            IcecatClientMock
                .Setup(x =>
                    x.GetProductByBrandAndSkuAsync(
                        "Zebra",
                        "SKU-WITHOUT-TITLE"))
                .ReturnsAsync(new IcecatProductResponse
                {
                    Data = new IcecatProductData
                    {
                        EssentialInfo = new IcecatEssentialInfo { ProductCode = "SKU-WITHOUT-TITLE" }
                    }
                });

            var result = await service.EnrichProductAsync(product.Id);

            result.Should().NotBeNull();

            result!.Name.Should().Be("Existing Icecat title");
            result.IcecatName.Should().Be("Existing Icecat title");

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.Name.Should().Be("Rackbeat product name");
            updated.IcecatName.Should().Be("Existing Icecat title");
        }

        [Fact]
        public async Task EnrichMissingContentAsync_Should_Enrich_Product_When_Only_IcecatName_Is_Missing()
        {
            var service = GetService<IProductContentEnrichmentService>();

            var brand = await CreateBrandAsync("Zebra");

            var product = await CreateProductAsync(
                name: "Rackbeat product name",
                sku: "SKU-MISSING-ICECAT-NAME",
                brandId: brand.Id);

            product.Description = "Existing description";

            product.SpecificationsJson = """
                [
                  {
                    "GroupName": "Design",
                    "Items": [
                      {
                        "Name": "Colour",
                        "Value": "Black"
                      }
                    ]
                  }
                ]
                """;

            Context.ProductImages.Add(
                new ProductImage
                {
                    ProductId = product.Id,
                    Url = "https://manual.test/image.jpg",
                    IsPrimary = true,
                    Source = "Manual"
                });

            await Context.SaveChangesAsync();

            IcecatClientMock
                .Setup(x =>
                    x.GetProductByBrandAndSkuAsync(
                        "Zebra",
                        "SKU-MISSING-ICECAT-NAME"))
                .ReturnsAsync(
                    IcecatTestDataFactory.CreateCompleteResponse(
                        title: "Complete Icecat product title",
                        productCode: "SKU-MISSING-ICECAT-NAME"));

            var result = await service.EnrichMissingContentAsync();

            result.Checked.Should().Be(1);
            result.FullyEnriched.Should().Be(1);
            result.PartiallyEnriched.Should().Be(0);
            result.Failed.Should().Be(0);

            ResetContext();

            var updated = await Context.Products.SingleAsync(x => x.Id == product.Id);

            updated.Name.Should().Be("Rackbeat product name");

            updated.IcecatName.Should().Be("Complete Icecat product title");
        }
    }
}
