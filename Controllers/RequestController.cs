using dotnet9.Dtos.Models;
using dotnet9.Dtos.Wrappers;
using dotnet9.Interfaces;
using dotnet9.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet9.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestRepository _requestRepo;
        private readonly IRequestMgmtService _requestMgmtService;
        public RequestController(IRequestRepository requestRepo, IRequestMgmtService requestMgmtService)
        {
            _requestRepo = requestRepo;
            _requestMgmtService = requestMgmtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _requestRepo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            return request == null ? NotFound() : Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromForm] CreateRequestDto createRequestDto)
        {
            try
            {
                var request = await _requestMgmtService.CreateRequestWithImagesAsync(createRequestDto.RequestDto, createRequestDto.ImageFiles!);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RequestDto requestDto)
        {
            if (id != requestDto.Id)
                return BadRequest("ID mismatch");

            var updated = await _requestRepo.UpdateAsync(requestDto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _requestMgmtService.DeleteRequestAndImagesAsync(id);
            return result ? Ok("Deleted") : BadRequest("Oops.");
        }
    }
}
