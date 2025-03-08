using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;
        public ImageRepository(AppDbContext context) => _context = context;

        private static ImageDto MapToDto(Image image) =>
            new ImageDto 
            { 
                Id = image.Id, 
                ParentId = image.ParentId, 
                ImageUrl = image.ImageUrl 
            };

        private static Image MapToEntity(ImageDto dto) =>
            new Image 
            { 
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                ParentId = dto.ParentId,
                ImageUrl = dto.ImageUrl
            };

        public async Task<IEnumerable<ImageDto>> GetAllAsync() =>
            await _context.Images
                .Select(i => new ImageDto 
                { 
                    Id = i.Id, 
                    ParentId = i.ParentId, 
                    ImageUrl = i.ImageUrl 
                })
                .ToListAsync();

        public async Task<ImageDto?> GetByIdAsync(Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            return image is null ? null : MapToDto(image);
        }

        public async Task<ImageDto> AddAsync(ImageDto ImageDto)
        {
            var Image = MapToEntity(ImageDto);
            _context.Images.Add(Image);
            await _context.SaveChangesAsync();
            return ImageDto;
        }

        public async Task<ImageDto?> UpdateAsync(ImageDto ImageDto)
        {
            var image = await _context.Images.FindAsync(ImageDto.Id);
            if (image is null)
                return null;
            
            image.ParentId = ImageDto.ParentId;
            image.ImageUrl = ImageDto.ImageUrl;
            _context.Images.Update(image);
            await _context.SaveChangesAsync();
            return MapToDto(image);
        }

        public async Task DeleteAsync(Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image is not null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
