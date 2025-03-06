
namespace dotnet9.Dtos.Models{
    public class ArticleImageDto
    {
        public Guid Id { get; set; }
        public required Guid ArticleId { get; set; }
        public required string ImageUrl { get; set; }
    }
}