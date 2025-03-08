using dotnet9.Dtos.Models;

namespace dotnet9.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task<IEnumerable<ImageDto>> GetAllAsync();
        Task<ImageDto?> GetByIdAsync(Guid id);
        Task<ImageDto> AddAsync(ImageDto ImageDto);
        Task<ImageDto?> UpdateAsync(ImageDto ImageDto);
        Task DeleteAsync(Guid id);
    }
}
