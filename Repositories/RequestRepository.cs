using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _context;
        public RequestRepository(AppDbContext context)
        {
            _context = context;
        }

        // Mapping from entity to DTO.
        private static RequestDto MapToDto(Request request) =>
            new RequestDto
            {
                Id = request.Id,
                Description = request.Description!,
                UserId = request.UserId,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                ArticleId = request.ArticleId,
                Quantity = request.Quantity
            };

        // Mapping from DTO to entity.
        private static Request MapToEntity(RequestDto dto) =>
            new Request
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Description = dto.Description,
                UserId = dto.UserId,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                ArticleId = dto.ArticleId,
                Quantity = dto.Quantity
            };

        public async Task<IEnumerable<RequestDto>> GetAllAsync() =>
            await _context.Requests
                          .Select(r => new RequestDto
                          {
                              Id = r.Id,
                              Description = r.Description!,
                              UserId = r.UserId,
                              CreatedAt = r.CreatedAt,
                              UpdatedAt = r.UpdatedAt,
                              ArticleId = r.ArticleId,
                              RequestImages = _context.Images
                                                .Where(i => i.ParentId == r.Id)
                                                .Select(i => i.ImageUrl)
                                                .ToList()
                          })
                          .ToListAsync();

        public async Task<RequestDto?> GetByIdAsync(Guid id)
        {
            var request = await _context.Requests.FindAsync(id);
            return request == null ? null : MapToDto(request);
        }

        public async Task<RequestDto> AddAsync(RequestDto requestDto)
        {
            var request = MapToEntity(requestDto);
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return MapToDto(request);
        }

        public async Task<RequestDto?> UpdateAsync(RequestDto requestDto)
        {
            var request = await _context.Requests.FindAsync(requestDto.Id);
            if (request == null)
                return null;
            
            request.Description = requestDto.Description;
            request.UserId = requestDto.UserId;
            request.ArticleId = requestDto.ArticleId;
            request.UpdatedAt = DateTime.UtcNow;
            // If needed, update images collection here.

            _context.Requests.Update(request);
            await _context.SaveChangesAsync();
            return MapToDto(request);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
                var res = await _context.SaveChangesAsync();
                return res > 0;
            }
            return false;
        }
    }
}
