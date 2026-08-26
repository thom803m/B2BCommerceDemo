using System.Text.Json.Serialization;

namespace B2BCommerceDemo.Core.DTOs.Integrations.Icecat
{
    public class IcecatProductResponse
    {
        [JsonPropertyName("data")]
        public IcecatProductData? Data { get; set; }
    }

    public class IcecatProductData
    {
        [JsonPropertyName("GeneralInfo")]
        public IcecatGeneralInfo? GeneralInfo { get; set; }

        [JsonPropertyName("EssentialInfo")]
        public IcecatEssentialInfo? EssentialInfo { get; set; }

        [JsonPropertyName("MarketingText")]
        public string? MarketingText { get; set; }

        [JsonPropertyName("SummaryDescription")]
        public IcecatSummaryDescription? SummaryDescription { get; set; }

        [JsonPropertyName("FeaturesGroups")]
        public List<IcecatFeaturesGroup> FeaturesGroups { get; set; } = new();

        [JsonPropertyName("Gallery")]
        public List<IcecatGalleryImage> Gallery { get; set; } = new();
    }

    public class IcecatGeneralInfo
    {
        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("TitleInfo")]
        public IcecatTitleInfo? TitleInfo { get; set; }
    }

    public class IcecatTitleInfo
    {
        [JsonPropertyName("GeneratedIntTitle")]
        public string? GeneratedIntTitle { get; set; }

        [JsonPropertyName("GeneratedLocalTitle")]
        public IcecatLocalizedValue?
            GeneratedLocalTitle
        { get; set; }

        [JsonPropertyName("BrandLocalTitle")]
        public IcecatLocalizedValue?
            BrandLocalTitle
        { get; set; }
    }

    public class IcecatEssentialInfo
    {
        [JsonPropertyName("ProductCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("ProductName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("Brand")]
        public string? Brand { get; set; }
    }

    public class IcecatSummaryDescription
    {
        [JsonPropertyName("SummaryDescription")]
        public string? SummaryDescription { get; set; }
    }

    public class IcecatFeaturesGroup
    {
        [JsonPropertyName("FeatureGroup")]
        public IcecatFeatureGroup? FeatureGroup { get; set; }

        [JsonPropertyName("Features")]
        public List<IcecatFeature> Features { get; set; } = new();
    }

    public class IcecatFeatureGroup
    {
        [JsonPropertyName("Name")]
        public IcecatLocalizedValue? Name { get; set; }
    }

    public class IcecatLocalizedValue
    {
        [JsonPropertyName("Value")]
        public string? Value { get; set; }

        [JsonPropertyName("Language")]
        public string? Language { get; set; }
    }

    public class IcecatFeature
    {
        [JsonPropertyName("Feature")]
        public IcecatFeatureInfo? Feature { get; set; }

        [JsonPropertyName("PresentationValue")]
        public string? PresentationValue { get; set; }
    }

    public class IcecatFeatureInfo
    {
        [JsonPropertyName("Name")]
        public IcecatLocalizedValue? Name { get; set; }
    }

    public class IcecatGalleryImage
    {
        [JsonPropertyName("ID")]
        public string? Id { get; set; }

        [JsonPropertyName("Pic")]
        public string? Pic { get; set; }

        [JsonPropertyName("Pic500x500")]
        public string? Pic500x500 { get; set; }

        [JsonPropertyName("ThumbPic")]
        public string? ThumbPic { get; set; }

        [JsonPropertyName("LowPic")]
        public string? LowPic { get; set; }

        [JsonPropertyName("HighPic")]
        public string? HighPic { get; set; }

        [JsonPropertyName("Original")]
        public string? Original { get; set; }

        [JsonPropertyName("IsMain")]
        public string? IsMain { get; set; }
    }
}

