
namespace dotnet9.Models{
    public class ArticleImage
    {
        public Guid Id { get; set; }
        public required Guid ArticleId { get; set; }
        public Article? Article { get; set; }
        public required string ImageUrl { get; set; }
    }
}