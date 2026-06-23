namespace EFCorePractice.DTOs;

public class ContactListItemDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
    public int TagCount { get; set; }
    public string? TagNames { get; set; }
}
