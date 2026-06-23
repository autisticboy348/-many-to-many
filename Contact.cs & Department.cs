using System.Collections.Generic;

namespace EFCorePractice.Models;

public class Contact
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsActive { get; set; }
    
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public List<ContactTag> ContactTags { get; set; } = new();
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
