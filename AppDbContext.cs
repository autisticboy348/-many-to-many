using Microsoft.EntityFrameworkCore;
using EFCorePractice.Models;

namespace EFCorePractice.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<ContactTag> ContactTags { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ContactTag>(entity =>
        {
            entity.HasKey(ct => new { ct.ContactId, ct.TagId });

            entity.HasOne(ct => ct.Contact)
                  .WithMany(c => c.ContactTags)
                  .HasForeignKey(ct => ct.ContactId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ct => ct.Tag)
                  .WithMany(t => t.ContactTags)
                  .HasForeignKey(ct => ct.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
