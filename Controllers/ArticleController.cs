using dotnet9.Dtos.Models;
using dotnet9.Interfaces;
using dotnet9.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnet9.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepo;
        private readonly IArticleMgmtService _articleMgmtService;
        public ArticleController(IArticleRepository articleRepo, IArticleMgmtService articleMgmtService)
        {
            _articleRepo = articleRepo;
            _articleMgmtService = articleMgmtService;
        }

        [HttpPost("withImages")]
        public async Task<IActionResult> CreateArticleWithImages([FromForm] ArticleDto articleDto, [FromForm] List<IFormFile> imageFiles)
        {
            try
            {
                var article = await _articleMgmtService.CreateArticleWithImagesAsync(articleDto, imageFiles);
                return Ok(article);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        
        [HttpGet("All")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _articleRepo.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> GetAllArticles() =>
            Ok(await _articleRepo.GetAllArticlesAsync());

        [HttpGet("Requests")]
        public async Task<IActionResult> GetAllRequests() =>
            Ok(await _articleRepo.GetAllRequestsAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var article = await _articleRepo.GetByIdAsync(id);
            return article is null ? NotFound() : Ok(article);
        }

        [HttpPost("Request")]
        public async Task<IActionResult> CreateRequest([FromBody] ArticleDto articleDto)
        {
            var created = await _articleRepo.AddAsync(articleDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ArticleUpdateDto articleDto)
        {
            if (id != articleDto.Id)
                return BadRequest("ID mismatch");
            
            var updated = await _articleRepo.UpdateAsync(articleDto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _articleMgmtService.DeleteArticleAndImagesAsync(id);
            if(res)
                return Ok("Deleted");
            return BadRequest("Oops.");
        }
    }
}
