using System.ComponentModel.DataAnnotations;

namespace dotnet9.Dtos.Account
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
