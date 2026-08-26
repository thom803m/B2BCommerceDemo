using B2BCommerceDemo.Core.DTOs.PriceGroups;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class PriceGroupService : IPriceGroupService
    {
        private readonly AppDbContext _context;

        public PriceGroupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PriceGroupDto>> GetAllAsync()
        {
            return await _context.PriceGroups
                .Select(pg => new PriceGroupDto
                {
                    Id = pg.Id,
                    Name = pg.Name,
                    PercentageAdjustment = pg.PercentageAdjustment
                })
                .ToListAsync();
        }

        public async Task<PriceGroupDto> UpdateAsync(int id, UpdatePriceGroupDto dto)
        {
            var group = await _context.PriceGroups
                .FirstOrDefaultAsync(pg => pg.Id == id);

            if (group == null)
            {
                throw new KeyNotFoundException($"Price group with id {id} was not found.");
            }

            group.Name = dto.Name;
            group.PercentageAdjustment = dto.PercentageAdjustment;

            await _context.SaveChangesAsync();

            return new PriceGroupDto
            {
                Id = group.Id,
                Name = group.Name,
                PercentageAdjustment = group.PercentageAdjustment
            };
        }
    }
}
