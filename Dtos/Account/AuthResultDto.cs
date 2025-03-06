using dotnet9.Dtos.Models;

namespace dotnet9.Dtos.Account
{
    public class AuthResultDto
    {
        public UserDto? User { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
