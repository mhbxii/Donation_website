using dotnet9.Dtos.Models;

namespace dotnet9.Repositories.Interfaces
{
    public interface IContactUsRepository
    {
        Task<IEnumerable<ContactUsDto>> GetAllAsync();
        Task<ContactUsDto?> GetByIdAsync(Guid id);
        Task<ContactUsDto> AddAsync(ContactUsDto contactUsDto);
        Task<ContactUsDto?> UpdateAsync(ContactUsDto contactUsDto);
        Task DeleteAsync(Guid id);
    }
}
