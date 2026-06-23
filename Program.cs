using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCorePractice.Data;
using EFCorePractice.Models;
using EFCorePractice.DTOs;

namespace EFCorePractice;

public class Program
{
    public static async Task Main(string[] args)
    {
        using AppDbContext context = new AppDbContext(new DbContextOptions<AppDbContext>());

        await SeedDataAsync(context);
        await PrintContactsAndTagsAsync(context);
        await FilterVipContactsAsync(context);

        await ScenarioA_OneLevelInclude(context);
        await ScenarioB_FullGraph(context);
        await ScenarioC_Projection(context);
        await ScenarioD_NPlusOneCorrected(context);
        await ScenarioE_SplitQuery(context);
    }

    private static async Task SeedDataAsync(AppDbContext context)
    {
        if (!await context.Tags.AnyAsync())
        {
            var tags = new List<Tag>
            {
                new() { Name = "VIP" },
                new() { Name = "Remote" },
                new() { Name = "Partner" },
                new() { Name = "Priority" }
            };
            await context.Tags.AddRangeAsync(tags);
            await context.SaveChangesAsync();
        }

        var contacts = await context.Contacts.Take(2).ToListAsync();
        var vipTag = await context.Tags.FirstAsync(t => t.Name == "VIP");
        var remoteTag = await context.Tags.FirstAsync(t => t.Name == "Remote");

        foreach (var contact in contacts)
        {
            if (!await context.ContactTags.AnyAsync(ct => ct.ContactId == contact.Id && ct.TagId == vipTag.Id))
            {
                context.ContactTags.Add(new ContactTag { ContactId = contact.Id, TagId = vipTag.Id });
            }
            if (!await context.ContactTags.AnyAsync(ct => ct.ContactId == contact.Id && ct.TagId == remoteTag.Id))
            {
                context.ContactTags.Add(new ContactTag { ContactId = contact.Id, TagId = remoteTag.Id });
            }
        }
        await context.SaveChangesAsync();
    }

    private static async Task PrintContactsAndTagsAsync(AppDbContext context)
    {
        var contacts = await context.Contacts
            .Include(c => c.ContactTags)
                .ThenInclude(ct => ct.Tag)
            .ToListAsync();

        Console.WriteLine("--- Список Контактов ---");
        foreach (var c in contacts)
        {
            var tagsText = c.ContactTags.Any()
                ? string.Join(", ", c.ContactTags.Select(ct => ct.Tag?.Name))
                : "(тегов нет)";
            Console.WriteLine($"{c.FirstName} {c.LastName} | {tagsText}");
        }

        var tags = await context.Tags
            .Include(t => t.ContactTags)
                .ThenInclude(ct => ct.Contact)
            .ToListAsync();

        Console.WriteLine("\n--- Список Тегов ---");
        foreach (var t in tags)
        {
            var contactsText = t.ContactTags.Any()
                ? string.Join(", ", t.ContactTags.Select(ct => $"{ct.Contact?.FirstName} {ct.Contact?.LastName}"))
                : "(контактов нет)";
            Console.WriteLine($"{t.Name} | {contactsText}");
        }
    }

    private static async Task FilterVipContactsAsync(AppDbContext context)
    {
        var vipActive = await context.Contacts
            .Where(c => c.IsActive && c.ContactTags.Any(ct => ct.Tag!.Name == "VIP"))
            .ToListAsync();
    }

    private static async Task ScenarioA_OneLevelInclude(AppDbContext context)
    {
        var contacts = await context.Contacts
            .Include(c => c.Department)
            .AsNoTracking()
            .ToListAsync();

        foreach (var c in contacts)
        {
            Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Department?.Name}");
        }
    }

    private static async Task ScenarioB_FullGraph(AppDbContext context)
    {
        var contacts = await context.Contacts
            .Include(c => c.Department)
            .Include(c => c.ContactTags)
                .ThenInclude(ct => ct.Tag)
            .ToListAsync();

        foreach (var c in contacts)
        {
            var tagsText = string.Join(", ", c.ContactTags.Select(ct => ct.Tag?.Name));
            Console.WriteLine($"{c.FirstName} | {c.Department?.Name} | {tagsText}");
        }
    }

    private static async Task ScenarioC_Projection(AppDbContext context)
    {
        var dtos = await context.Contacts
            .Select(c => new ContactListItemDto
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                DepartmentName = c.Department != null ? c.Department.Name : "Нет отдела",
                TagCount = c.ContactTags.Count
            })
            .ToListAsync();
    }

    private static async Task ScenarioD_NPlusOneCorrected(AppDbContext context)
    {
        var contactsFixed = await context.Contacts
            .Include(c => c.Department)
            .ToListAsync();
    }

    private static async Task ScenarioE_SplitQuery(AppDbContext context)
    {
        var contacts = await context.Contacts
            .Include(c => c.Department)
            .Include(c => c.ContactTags)
                .ThenInclude(ct => ct.Tag)
            .AsSplitQuery()
            .ToListAsync();
    }
}
