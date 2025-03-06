using dotnet9.Dtos.Models;

namespace dotnet9.Interfaces{
    public interface ISetUserImageService{
        public Task<UserDto?> SetUserImageAsync(Guid userId, IFormFile file);
    }
}