using dotnet9.Dtos.Models;

namespace dotnet9.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task<CategoryDto> AddAsync(CategoryDto categoryDto);
        Task<CategoryDto?> UpdateAsync(CategoryDto categoryDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
