using dotnet9.Models;

namespace dotnet9.Interfaces
{
    public interface ITokenService{
        string CreateToken(User user);
    }
}