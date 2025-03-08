using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Interfaces;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using dotnet9.Services;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _context;
        private readonly IRequestMgmtService _requestMgmtService;
        public ArticleRepository(AppDbContext context, IRequestMgmtService requestMgmtService){
            _context = context;
            _requestMgmtService = requestMgmtService;
        }
        
        // Map from entity to DTO.
        private static ArticleDto MapToDto(Article article) =>
            new ArticleDto 
            { 
                Id = article.Id,
                Title = article.Title,
                Location = article.Location,
                Quantity = article.Quantity,
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
                Quantity = dto.Quantity,
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
                .Where(a => a.Quantity > 0)
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.Requests)
                .Select(a => new ArticleInfoDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Location = a.Location,
                    Description = a.Description ?? string.Empty,
                    Quantity = a.Quantity,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UserName = a.User!.UserName!,
                    Category = a.Category!.Name
                    
                    // Project requests explicitly into DTOs (or just force materialization)
                    /*Requests = a.Requests!.Select(r => new RequestDto
                    {
                        Id = r.Id,
                        Description = r.Description!,
                        Quantity = r.Quantity,
                        UserId = r.UserId,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        ArticleId = r.ArticleId,
                        RequestImages = _context.Images
                            .Where(i => i.ParentId == a.Id)
                            .Select(i => i.ImageUrl)
                            .ToList()
                        // If you need images, you can add that projection here too.
                    }).ToList()*/
              
                    ,Images = _context.Images
                                .Where(i => i.ParentId == a.Id)
                                .Select(i => i.ImageUrl)
                                .ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ArticleInfoDto>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Articles
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.Requests!) // Include Requests first
                    .ThenInclude(r => r.User) // ✅ Include User inside Requests
                .Select(a => new ArticleInfoDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Location = a.Location,
                    Description = a.Description ?? string.Empty,
                    Quantity = a.Quantity,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UserName = a.User!.UserName!, // Article owner
                    Category = a.Category!.Name,
                    Requests = a.Requests!.Select(r => new RequestInfoDto
                    {
                        Id = r.Id,
                        Description = r.Description!,
                        Quantity = r.Quantity,
                        Username = r.User != null ? r.User.UserName : "Unknown", // ✅ Get requester's username safely
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        ArticleId = r.ArticleId,
                        RequestImages = _context.Images
                            .Where(i => i.ParentId == r.Id)
                            .Select(i => i.ImageUrl)
                            .ToList()
                    }).ToList(),
                    Images = _context.Images
                        .Where(i => i.ParentId == a.Id)
                        .Select(i => i.ImageUrl)
                        .ToList()
                })
                .ToListAsync();
        }


        public async Task<ArticleInfoDto?> GetByIdAsync(Guid id)
        {
            return await _context.Articles
                .Where(a =>a.Quantity > 0 && a.Id == id)
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.ArticleImages)
                .Select(a => new ArticleInfoDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Location = a.Location,
                    Description = a.Description ?? string.Empty,
                    Quantity = a.Quantity,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    UserName = a.User!.UserName!,
                    Category = a.Category!.Name,
                    Requests = a.Requests!.Select(r => new RequestInfoDto
                    {
                        Id = r.Id,
                        Description = r.Description!,
                        Quantity = r.Quantity,
                        Username = r.User!.UserName,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        ArticleId = r.ArticleId,
                        RequestImages = _context.Images
                            .Where(i => i.ParentId == a.Id)
                            .Select(i => i.ImageUrl)
                            .ToList()
                        // If you need images, you can add that projection here too.
                    }).ToList(),
                    Images = _context.Images
                            .Where(i => i.ParentId == a.Id)
                            .Select(i => i.ImageUrl)
                            .ToList()
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

        public async Task<bool> AcceptRequestAsync(Guid articleId, Guid requestId)
        {
            var article = await _context.Articles
                .Include(a => a.Requests) // Ensure Requests are loaded
                .FirstOrDefaultAsync(a => a.Id == articleId);

            if (article == null || article.Requests == null)
                return false;

            // Check if the request exists in the article's requests
            if (!article.Requests.Any(r => r.Id == requestId))
                return false;

            var request = await _context.Requests.FindAsync(requestId);
            if (request == null)
                return false;

            if(article.Quantity < request.Quantity)
                return false;

            article.Quantity -= request.Quantity;   
            await _context.SaveChangesAsync();

            var res = await _requestMgmtService.DeleteRequestAndImagesAsync(requestId);
            return res;
        }
    }
}
