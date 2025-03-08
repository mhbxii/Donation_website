
namespace dotnet9.Dtos.Models{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Location { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }  // true = donation, false = request
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
    }
}