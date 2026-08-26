using B2BCommerceDemo.Core.DTOs.Companies;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class CompanyPriceService : ICompanyPriceService
    {
        private readonly AppDbContext _context;

        public CompanyPriceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyPriceDto>> GetAllAsync()
        {
            return await _context.CompanyPrices
                .Select(cp => new CompanyPriceDto
                {
                    Id = cp.Id,
                    ProductId = cp.ProductId,
                    CompanyId = cp.CompanyId,
                    Price = cp.Price
                })
                .ToListAsync();
        }

        public async Task<CompanyPriceDto> CreateAsync(CreateCompanyPriceDto dto)
        {
            var entity = new CompanyPrice
            {
                ProductId = dto.ProductId,
                CompanyId = dto.CompanyId,
                Price = dto.Price
            };

            _context.CompanyPrices.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("This company price already exists for this product.");
            }

            return new CompanyPriceDto
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                CompanyId = entity.CompanyId,
                Price = entity.Price
            };
        }

        public async Task<CompanyPriceDto> UpdateAsync(int id, UpdateCompanyPriceDto dto)
        {
            var entity = await _context.CompanyPrices.FindAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Company price with id {id} was not found.");
            }

            entity.Price = dto.Price;

            await _context.SaveChangesAsync();

            return new CompanyPriceDto
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                CompanyId = entity.CompanyId,
                Price = entity.Price
            };
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.CompanyPrices.FindAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Company price with id {id} was not found.");
            }

            _context.CompanyPrices.Remove(entity);

            await _context.SaveChangesAsync();
        }
    }
}
