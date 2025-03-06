using dotnet9.Dtos.Models;

namespace dotnet9.Interfaces
{
    public interface IArticleMgmtService
    {
        Task<ArticleDto> CreateArticleWithImagesAsync(ArticleDto articleDto, List<IFormFile> imageFiles);
        Task<bool> DeleteArticleAndImagesAsync(Guid articleId);
    }
}
