using dotnet9.Dtos.Models;

namespace dotnet9.Dtos.Wrappers{
    public class CreateRequestDto{
        public required RequestDto RequestDto { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
}