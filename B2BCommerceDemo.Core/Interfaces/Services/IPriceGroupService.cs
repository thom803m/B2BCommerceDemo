using B2BCommerceDemo.Core.DTOs.PriceGroups;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IPriceGroupService
    {
        Task<List<PriceGroupDto>> GetAllAsync();
        Task<PriceGroupDto> UpdateAsync(int id, UpdatePriceGroupDto dto);
    }
}
