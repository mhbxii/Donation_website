using Microsoft.AspNetCore.Identity;

namespace dotnet9.Models
{
    public class User : IdentityUser<Guid>{
        public DateTime CreatedAt {get; set;}
        public string? RefreshToken {get; set;}
        public DateTime RefreshTokenExpiryTime {get; set;}
        public string? Origin {get; set;}
        public string? ImageUrl {get; set;}
        public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Article>? Articles {get; set;}
        public ICollection<ContactUs>? ContactUs {get; set;}
    }
}