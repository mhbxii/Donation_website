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
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepo;
        private readonly IArticleMgmtService _articleMgmtService;
        public ArticleController(IArticleRepository articleRepo, IArticleMgmtService articleMgmtService)
        {
            _articleRepo = articleRepo;
            _articleMgmtService = articleMgmtService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticleWithImages([FromForm] CreateArticletDto dto)
        {
            try
            {
                var article = await _articleMgmtService.CreateArticleWithImagesAsync(dto.ArticleDto, dto.ImageFiles!);
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

        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetByUserId(Guid userId){
            return Ok(await _articleRepo.GetByUserIdAsync(userId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var article = await _articleRepo.GetByIdAsync(id);
            return article is null ? NotFound() : Ok(article);
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

        [HttpPost("{articleId}/AcceptRequest/{requestId}")]
        public async Task<IActionResult> AcceptRequest([FromRoute] Guid articleId, [FromRoute] Guid requestId){

            var res = await _articleRepo.AcceptRequestAsync(articleId, requestId);

            if(res)
                return Ok("Success!");
            return BadRequest("Oops.");
        }
    }
}
