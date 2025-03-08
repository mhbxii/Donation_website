
namespace dotnet9.Models{
    public class Image
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
        
        // Polymorphic association
        public Guid ParentId { get; set; }
    }
}