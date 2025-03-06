using System.ComponentModel.DataAnnotations;

namespace dotnet9.Dtos.Account
{
    public class NewUserDto{
        [Required]
        public Guid? Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}