using System.ComponentModel.DataAnnotations;

namespace dotnet9.Dtos.Account
{
    public class RegisterDto{
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        public string? Origin {get; set;}
        public IFormFile? ImageFile { get; set; }
    }
}