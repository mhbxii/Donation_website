
using System.ComponentModel.DataAnnotations;

namespace dotnet9.Dtos.Models
{
    public class UserDto{
        public Guid? Id { get; set; }
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Origin {get; set;}
        public string? ImageUrl {get; set;}
        public string? PhoneNumber {get; set;}
    }
}