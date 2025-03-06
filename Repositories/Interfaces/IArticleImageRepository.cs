using dotnet9.Dtos.Models;

namespace dotnet9.Repositories.Interfaces
{
    public interface IArticleImageRepository
    {
        Task<IEnumerable<ArticleImageDto>> GetAllAsync();
        Task<ArticleImageDto?> GetByIdAsync(Guid id);
        Task<ArticleImageDto> AddAsync(ArticleImageDto articleImageDto);
        Task<ArticleImageDto?> UpdateAsync(ArticleImageDto articleImageDto);
        Task DeleteAsync(Guid id);
    }
}
