using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _context;
        public ArticleRepository(AppDbContext context){
            _context = context;
        }
        
        // Map from entity to DTO.
        private static ArticleDto MapToDto(Article article) =>
            new ArticleDto 
            { 
                Id = article.Id,
                Title = article.Title,
                Location = article.Location,
                IsDonation = article.IsDonation,
                Description = article.Description!,
                UserId = article.UserId,
                CategoryId = article.CategoryId,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt
            };

        // Map from DTO to entity.
        private static Article MapToEntity(ArticleDto dto)
        {      
            return new Article 
            { 
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Title = dto.Title,
                Location = dto.Location,
                IsDonation = dto.IsDonation,
                Description = dto.Description,
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public async Task<IEnumerable<ArticleInfoDto>> GetAllAsync()
        {
            return await _context.Articles
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.ArticleImages)
                .Select(a => new ArticleInfoDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Location = a.Location,
                    Description = a.Description ?? string.Empty,
                    IsDonation = a.IsDonation,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UserName = a.User!.UserName!,        // Adjust property name if needed
                    Category = a.Category!.Name,      // Adjust property name if needed
                    Images = a.ArticleImages!.Select(ai => ai.ImageUrl).ToList()
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<ArticleInfoDto>> GetAllArticlesAsync()
        {
            return await _context.Articles
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.ArticleImages)
                .Where(a => a.IsDonation)
                .Select(a => new ArticleInfoDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Location = a.Location,
                    Description = a.Description ?? string.Empty,
                    IsDonation = a.IsDonation,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UserName = a.User!.UserName!,        // Adjust property name if needed
                    Category = a.Category!.Name,      // Adjust property name if needed
                    Images = a.ArticleImages!.Select(ai => ai.ImageUrl).ToList()
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<ArticleInfoDto>> GetAllRequestsAsync()
        {
            return await _context.Articles
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.ArticleImages)
                .Where(a => !a.IsDonation)
                .Select(a => new ArticleInfoDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Location = a.Location,
                    Description = a.Description ?? string.Empty,
                    IsDonation = a.IsDonation,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UserName = a.User!.UserName!,        // Adjust property name if needed
                    Category = a.Category!.Name,      // Adjust property name if needed
                    Images = a.ArticleImages!.Select(ai => ai.ImageUrl).ToList()
                })
                .ToListAsync();
        }

        public async Task<ArticleInfoDto?> GetByIdAsync(Guid id)
        {
            return await _context.Articles
                .Where(a => a.Id == id)
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.ArticleImages)
                .Select(a => new ArticleInfoDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Location = a.Location,
                    Description = a.Description ?? string.Empty,
                    IsDonation = a.IsDonation,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UserName = a.User!.UserName!,
                    Category = a.Category!.Name,
                    Images = a.ArticleImages!.Select(ai => ai.ImageUrl).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ArticleDto> AddAsync(ArticleDto articleDto)
        {   
            var article = MapToEntity(articleDto);
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return MapToDto(article);
        }

        public async Task<ArticleDto?> UpdateAsync(ArticleUpdateDto articleUpdateDto)
        {
            var article = await _context.Articles.FindAsync(articleUpdateDto.Id);
            if (article is null)
                return null;
            
            article.UpdatedAt = DateTime.UtcNow;
            article.Title = articleUpdateDto.Title;
            article.Location = articleUpdateDto.Location;
            article.Description = articleUpdateDto.Description;

            if (articleUpdateDto.CategoryId != Guid.Empty)
            {
                // Optional: Validate that the Category exists.
                if (!await _context.Categories.AnyAsync(c => c.Id == articleUpdateDto.CategoryId))
                {
                    throw new Exception("Invalid CategoryId provided.");
                }
                article.CategoryId = articleUpdateDto.CategoryId;
            }
            
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
            return MapToDto(article);
        }
    }
}
