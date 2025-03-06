using System.Security.Claims;
using dotnet9.Dtos.Account;
using dotnet9.Dtos.Models;

namespace dotnet9.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal principal);
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task RequestResetPasswordAsync(ResetPasswordRequestDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}