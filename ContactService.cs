namespace EFCorePractice.Services;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCorePractice.Data;
using EFCorePractice.DTOs;

public class ContactService
{
    private readonly AppDbContext _context;

    public ContactService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContactListItemDto?> GetContactCardEagerAsync(int id)
    {
        var contact = await _context.Contacts
            .Include(c => c.Department)
            .Include(c => c.ContactTags)
                .ThenInclude(ct => ct.Tag)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null) return null;

        return new ContactListItemDto
        {
            Id = contact.Id,
            FullName = $"{contact.FirstName} {contact.LastName}",
            DepartmentName = contact.Department?.Name ?? "Нет отдела",
            TagCount = contact.ContactTags.Count,
            TagNames = string.Join(", ", contact.ContactTags.Select(ct => ct.Tag?.Name))
        };
    }

    public async Task<ContactListItemDto?> GetContactCardProjectionAsync(int id)
    {
        return await _context.Contacts
            .Where(c => c.Id == id)
            .Select(c => new ContactListItemDto
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                DepartmentName = c.Department != null ? c.Department.Name : "Нет отдела",
                TagCount = c.ContactTags.Count,
                TagNames = string.Join(", ", c.ContactTags.Select(ct => ct.Tag!.Name))
            })
            .FirstOrDefaultAsync();
    }
}
