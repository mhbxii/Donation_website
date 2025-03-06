
namespace dotnet9.Dtos.Models{
    public class ContactUsDto
    {
        public Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string TextContent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}