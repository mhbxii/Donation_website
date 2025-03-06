using dotnet9.Dtos.Models;
using dotnet9.Interfaces;
using dotnet9.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnet9.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleImagesController : ControllerBase
    {
        private readonly IArticleImageRepository _imageRepo;
        public ArticleImagesController(IArticleImageRepository imageRepo){
            _imageRepo = imageRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _imageRepo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var image = await _imageRepo.GetByIdAsync(id);
            return image is null ? NotFound() : Ok(image);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ArticleImageDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");
            
            var updated = await _imageRepo.UpdateAsync(dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _imageRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
