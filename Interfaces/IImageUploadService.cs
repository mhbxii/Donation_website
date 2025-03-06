using dotnet9.Dtos.Models;

namespace dotnet9.Interfaces
{
    public interface IImageUploadService
    {
        Task<string?> UploadImageAsync(IFormFile file);
    }
}
