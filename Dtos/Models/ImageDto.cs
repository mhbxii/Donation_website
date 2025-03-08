
namespace dotnet9.Dtos.Models{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
        public Guid ParentId { get; set; }
    }
}