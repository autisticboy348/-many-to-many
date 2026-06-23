using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCorePractice.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = null!;

    public List<ContactTag> ContactTags { get; set; } = new();
}
