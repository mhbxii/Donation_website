
namespace dotnet9.Models{
    public class ContactUs
    {
        public Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public User? User { get; set; }
        public required string TextContent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}