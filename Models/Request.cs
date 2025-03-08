
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet9.Models{
    public class Request
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ArticleId { get; set; }

        //Navigation for user
        public User? User { get; set; }
        //Navigation for Article:
        public Article? Article { get; set; }
        // Navigation property for images.
        [NotMapped]
        public ICollection<Image>? RequestImages { get; set; } = [];
    }

}