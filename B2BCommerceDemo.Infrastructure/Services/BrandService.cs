using B2BCommerceDemo.Core.DTOs.Brands;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.Infrastructure.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;
        private readonly IValidateUniqueness _validateUniqueness;

        public BrandService(AppDbContext context, IValidateUniqueness validateUniqueness)
        {
            _context = context;
            _validateUniqueness = validateUniqueness;
        }

        public async Task<List<BrandDto>> GetBrandsAsync()
        {
            return await _context.Brands
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .Select(b => new BrandDto { Id = b.Id, Name = b.Name })
                .ToListAsync();
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            return new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto dto)
        {
            var name = dto.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Brand name cannot be empty");
            }

            await _validateUniqueness.ValidateUniqueBrandNameAsync(name);

            var brand = new Brand { Name = name };

            _context.Brands.Add(brand);

            await _context.SaveChangesAsync();

            return new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task<BrandDto?> UpdateBrandAsync(int id, UpdateBrandDto dto)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            var name = dto.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Brand name cannot be empty");
            } 

            await _validateUniqueness.ValidateUniqueBrandNameAsync(name, id);

            brand.Name = name;

            await _context.SaveChangesAsync();

            return new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            _context.Brands.Remove(brand);

            await _context.SaveChangesAsync();
        }
    }
}

