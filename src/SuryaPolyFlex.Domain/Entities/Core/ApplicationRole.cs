using Microsoft.AspNetCore.Identity;


namespace SuryaPolyFlex.Domain.Entities.Core;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}