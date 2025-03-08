using dotnet9.Dtos.Models;

namespace dotnet9.Repositories.Interfaces
{
    public interface IRequestRepository
    {
        Task<IEnumerable<RequestDto>> GetAllAsync();
        Task<RequestDto?> GetByIdAsync(Guid id);
        Task<RequestDto> AddAsync(RequestDto requestDto);
        Task<RequestDto?> UpdateAsync(RequestDto requestDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
