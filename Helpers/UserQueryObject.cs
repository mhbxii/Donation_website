namespace dotnet9.Helpers
{
    public class UserQueryObject{
        public string? userName {get; set;} = null;
        public string? SortBy { get; set; } = null;
        public bool Desc { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}