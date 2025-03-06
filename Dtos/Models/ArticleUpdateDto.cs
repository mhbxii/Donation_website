
namespace dotnet9.Dtos.Models{
    public class ArticleUpdateDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Location { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}