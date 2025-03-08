
using dotnet9.Models;

namespace dotnet9.Dtos.Models{
    public class RequestDto
    {
        public Guid Id { get; set; }
        public required string Description { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ArticleId { get; set; }
        public int Quantity { get; set; }

        // Navigation property for images.
        public ICollection<string>? RequestImages { get; set; } = [];
    }

}