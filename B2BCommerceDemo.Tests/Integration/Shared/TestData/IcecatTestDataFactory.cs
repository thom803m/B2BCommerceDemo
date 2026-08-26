using B2BCommerceDemo.Core.DTOs.Integrations.Icecat;

namespace B2BCommerceDemo.Tests.Integration.Shared.TestData
{
    public static class IcecatTestDataFactory
    {
        public static IcecatProductResponse CreateCompleteResponse(
            string title = "Test Icecat product",
            string productCode = "ICECAT-123",
            string description = "Icecat description")
        {
            return new IcecatProductResponse
            {
                Data = new IcecatProductData
                {
                    GeneralInfo = new IcecatGeneralInfo
                    {
                        Title = title
                    },

                    EssentialInfo = new IcecatEssentialInfo
                    {
                        ProductCode = productCode
                    },

                    SummaryDescription =
                        new IcecatSummaryDescription
                        {
                            SummaryDescription = description
                        },

                    FeaturesGroups =
                    [
                        new IcecatFeaturesGroup
                        {
                            FeatureGroup =
                                new IcecatFeatureGroup
                                {
                                    Name =
                                        new IcecatLocalizedValue
                                        {
                                            Value = "Design",
                                            Language = "EN"
                                        }
                                },

                            Features =
                            [
                                new IcecatFeature
                                {
                                    Feature =
                                        new IcecatFeatureInfo
                                        {
                                            Name =
                                                new IcecatLocalizedValue
                                                {
                                                    Value = "Product colour",
                                                    Language = "EN"
                                                }
                                        },

                                    PresentationValue = "Black"
                                }
                            ]
                        }
                    ]
                }
            };
        }

        public static IcecatProductResponse CreateResponseWithImages(
            string productCode = "ICECAT-123",
            string description = "Icecat description")
        {
            var response = CreateCompleteResponse(
                productCode: productCode,
                description: description);

            response.Data!.Gallery =
            [
                new IcecatGalleryImage
                {
                    Id = "IMG-1",
                    Pic500x500 = "https://images.icecat.biz/img1.jpg",
                    IsMain = "Y"
                },
                new IcecatGalleryImage
                {
                    Id = "IMG-2",
                    Pic500x500 = "https://images.icecat.biz/img2.jpg",
                    IsMain = "N"
                }
            ];

            return response;
        }

        public static IcecatProductResponse CreateEmptyResponse()
        {
            return new IcecatProductResponse
            {
                Data = new IcecatProductData()
            };
        }

        public static IcecatProductResponse
            CreateResponseWithGeneratedLocalTitle(
                string generatedTitle,
                string productCode = "ICECAT-123")
        {
            var response = CreateCompleteResponse(
                title: "",
                productCode: productCode);

            response.Data!.GeneralInfo!.TitleInfo =
                new IcecatTitleInfo
                {
                    GeneratedLocalTitle =
                        new IcecatLocalizedValue
                        {
                            Value = generatedTitle,
                            Language = "EN"
                        }
                };

            return response;
        }
    }
}
