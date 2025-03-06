using dotnet9.Dtos.Models;

namespace dotnet9.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<IEnumerable<ArticleInfoDto>> GetAllAsync();
        Task<IEnumerable<ArticleInfoDto>> GetAllArticlesAsync();
        Task<IEnumerable<ArticleInfoDto>> GetAllRequestsAsync();
        Task<ArticleInfoDto?> GetByIdAsync(Guid id);
        Task<ArticleDto> AddAsync(ArticleDto articleDto);
        Task<ArticleDto?> UpdateAsync(ArticleUpdateDto articleDto);
        //Task<bool> DeleteAsync(Guid id);
    }
}
