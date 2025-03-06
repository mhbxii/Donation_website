
using dotnet9.Dtos.Models;
using dotnet9.Helpers;
using dotnet9.Models;

namespace dotnet9.Repositories.Interfaces
{
    public interface IUserRepository{
        Task<List<UserDto>> GetAllUsersWithAsync(UserQueryObject userQueryable);
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<bool> UpdateUser(Guid id, User NewUser);
        Task<bool> DeleteUser(Guid id);
    }
}