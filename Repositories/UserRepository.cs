using dotnet9.Data;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using dotnet9.Helpers;
using dotnet9.Dtos.Models;
namespace dotnet9.Repositories
{
    public class UserRepository : IUserRepository{
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext AppDbContext)
        {
            _context = AppDbContext;
        }
        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
                return null;
            return new UserDto{
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Origin = user.Origin,
                ImageUrl = user.ImageUrl,
            };
        }
        public async Task<bool> UpdateUser(Guid id, User NewUser)
        {
            if (NewUser == null || NewUser.Id != id)
                return false;
            var oldUser = await _context.Users.FindAsync(id);
            if (oldUser == null)
                return false;
           
            oldUser.Email = NewUser.Email;
            oldUser.UserName = NewUser.UserName;
            var res = await _context.SaveChangesAsync();
            return res >0 ;
        }
        public async Task<bool> DeleteUser(Guid id)
        {
            var userTarget = await _context.Users.FindAsync(id);
            if(userTarget is null)
                return false;
           
            _context.Users.Remove(userTarget);
            var res = await _context.SaveChangesAsync();
            return res > 0;
        }
        public async Task<List<UserDto>> GetAllUsersWithAsync(UserQueryObject query)
        {
            var listOfUsers = _context.Users.AsQueryable();
           
            if(!string.IsNullOrWhiteSpace(query.userName)){
                query.userName = query.userName.Trim();
                listOfUsers = listOfUsers.Where(u => u.UserName!.ToLower().Contains(query.userName.ToLower()));
            }
           
            if(!string.IsNullOrWhiteSpace(query.SortBy)){
                if(query.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase)){
                    listOfUsers = query.Desc ? listOfUsers.OrderByDescending(ph => ph.UserName) : listOfUsers.OrderBy(ph => ph.UserName);
                }
            }
            return await listOfUsers
                .Select(user => new UserDto{
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Origin = user.Origin,
                    ImageUrl = user.ImageUrl,
                })
                .ToListAsync();
        }
    }
}