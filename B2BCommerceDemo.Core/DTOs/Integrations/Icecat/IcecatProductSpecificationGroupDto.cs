namespace B2BCommerceDemo.Core.DTOs.Integrations.Icecat
{
    public class IcecatProductSpecificationGroupDto
    {
        public string GroupName { get; set; } = "";
        public List<IcecatProductSpecificationItemDto> Items { get; set; } = new();
    }

    public class IcecatProductSpecificationItemDto
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}

