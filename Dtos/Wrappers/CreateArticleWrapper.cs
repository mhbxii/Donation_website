using dotnet9.Dtos.Models;

namespace dotnet9.Dtos.Wrappers{
    public class CreateArticletDto{
        public required ArticleDto ArticleDto { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
}