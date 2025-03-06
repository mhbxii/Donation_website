using dotnet9.Dtos.Models;
using dotnet9.Repositories;
using dotnet9.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnet9.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsRepository _contactRepo;
        public ContactUsController(IContactUsRepository contactRepo) => _contactRepo = contactRepo;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _contactRepo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var contact = await _contactRepo.GetByIdAsync(id);
            return contact is null ? NotFound() : Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactUsDto dto)
        {
            var created = await _contactRepo.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ContactUsDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");
            
            var updated = await _contactRepo.UpdateAsync(dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _contactRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
