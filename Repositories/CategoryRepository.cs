using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        // Map from entity to DTO.
        private static CategoryDto MapToDto(Category category){
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        // Map from DTO to entity.
        private static Category MapToEntity(CategoryDto dto){
            return new Category
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Name = dto.Name
            };
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(){
            return await _context.Categories
                          .Select(c => new CategoryDto
                          {
                              Id = c.Id,
                              Name = c.Name,
                              
                          })
                          .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category is null ? null : MapToDto(category);
        }

        public async Task<CategoryDto> AddAsync(CategoryDto categoryDto)
        {
            var category = MapToEntity(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateAsync(CategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(categoryDto.Id);
            if (category is null)
                return null;

            category.Name = categoryDto.Name;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is not null)
            {
                _context.Categories.Remove(category);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}
