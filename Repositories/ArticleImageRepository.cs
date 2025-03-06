using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Repositories
{
    public class ArticleImageRepository : IArticleImageRepository
    {
        private readonly AppDbContext _context;
        public ArticleImageRepository(AppDbContext context) => _context = context;

        private static ArticleImageDto MapToDto(ArticleImage image) =>
            new ArticleImageDto 
            { 
                Id = image.Id, 
                ArticleId = image.ArticleId, 
                ImageUrl = image.ImageUrl 
            };

        private static ArticleImage MapToEntity(ArticleImageDto dto) =>
            new ArticleImage 
            { 
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                ArticleId = dto.ArticleId,
                ImageUrl = dto.ImageUrl
            };

        public async Task<IEnumerable<ArticleImageDto>> GetAllAsync() =>
            await _context.ArticleImages
                .Select(i => new ArticleImageDto 
                { 
                    Id = i.Id, 
                    ArticleId = i.ArticleId, 
                    ImageUrl = i.ImageUrl 
                })
                .ToListAsync();

        public async Task<ArticleImageDto?> GetByIdAsync(Guid id)
        {
            var image = await _context.ArticleImages.FindAsync(id);
            return image is null ? null : MapToDto(image);
        }

        public async Task<ArticleImageDto> AddAsync(ArticleImageDto articleImageDto)
        {
            var articleImage = MapToEntity(articleImageDto);
            _context.ArticleImages.Add(articleImage);
            await _context.SaveChangesAsync();
            return articleImageDto;
        }

        public async Task<ArticleImageDto?> UpdateAsync(ArticleImageDto articleImageDto)
        {
            var image = await _context.ArticleImages.FindAsync(articleImageDto.Id);
            if (image is null)
                return null;
            
            image.ArticleId = articleImageDto.ArticleId;
            image.ImageUrl = articleImageDto.ImageUrl;
            _context.ArticleImages.Update(image);
            await _context.SaveChangesAsync();
            return MapToDto(image);
        }

        public async Task DeleteAsync(Guid id)
        {
            var image = await _context.ArticleImages.FindAsync(id);
            if (image is not null)
            {
                _context.ArticleImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
