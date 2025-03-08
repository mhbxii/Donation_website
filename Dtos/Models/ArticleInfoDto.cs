
using dotnet9.Models;

namespace dotnet9.Dtos.Models{
    public class ArticleInfoDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Location { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public ICollection<string>? Images { get; set; } = [];
        public ICollection<RequestInfoDto>? Requests { get; set; } = [];
    }
}