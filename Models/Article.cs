
namespace dotnet9.Models{
    public class Article
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Location { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }  // true = donation, false = request
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid CategoryId { get; set; }
        
        //Navigation for category:
        public Category? Category { get; set; }

        // Navigation property for images.
        public ICollection<Image>? ArticleImages { get; set; } = [];
        // Navigation property for requests.
        public ICollection<Request>? Requests { get; set; } = [];
    }

}