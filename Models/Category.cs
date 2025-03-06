namespace dotnet9.Models{
    public class Category{
        public required Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        //Navigation to Articles:
        public ICollection<Article>? Articles { get; set; } = new List<Article>(){};
    }
}