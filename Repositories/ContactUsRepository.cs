using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Repositories
{
    public class ContactUsRepository : IContactUsRepository
    {
        private readonly AppDbContext _context;
        public ContactUsRepository(AppDbContext context) => _context = context;

        private static ContactUsDto MapToDto(ContactUs contact) =>
            new ContactUsDto 
            { 
                Id = contact.Id,
                UserId = contact.UserId,
                TextContent = contact.TextContent,
                CreatedAt = contact.CreatedAt
            };

        private static ContactUs MapToEntity(ContactUsDto dto) =>
            new ContactUs 
            { 
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                UserId = dto.UserId,
                TextContent = dto.TextContent,
                CreatedAt = DateTime.UtcNow
            };

        public async Task<IEnumerable<ContactUsDto>> GetAllAsync() =>
            await _context.ContactUs
                .Select(c => new ContactUsDto 
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    TextContent = c.TextContent,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

        public async Task<ContactUsDto?> GetByIdAsync(Guid id)
        {
            var contact = await _context.ContactUs.FindAsync(id);
            return contact is null ? null : MapToDto(contact);
        }

        public async Task<ContactUsDto> AddAsync(ContactUsDto contactUsDto)
        {
            var contact = MapToEntity(contactUsDto);
            _context.ContactUs.Add(contact);
            await _context.SaveChangesAsync();
            return MapToDto(contact);
        }

        public async Task<ContactUsDto?> UpdateAsync(ContactUsDto contactUsDto)
        {
            var contact = await _context.ContactUs.FindAsync(contactUsDto.Id);
            if (contact is null)
                return null;
            
            contact.TextContent = contactUsDto.TextContent;
            _context.ContactUs.Update(contact);
            await _context.SaveChangesAsync();
            return MapToDto(contact);
        }

        public async Task DeleteAsync(Guid id)
        {
            var contact = await _context.ContactUs.FindAsync(id);
            if (contact is not null)
            {
                _context.ContactUs.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }
    }
}
