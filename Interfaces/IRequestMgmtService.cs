using dotnet9.Dtos.Models;

namespace dotnet9.Interfaces
{
    public interface IRequestMgmtService
    {
        Task<RequestDto> CreateRequestWithImagesAsync(RequestDto RequestDto, List<IFormFile> imageFiles);
        Task<bool> DeleteRequestAndImagesAsync(Guid RequestId);
    }
}
