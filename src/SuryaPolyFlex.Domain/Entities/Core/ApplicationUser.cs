using Microsoft.AspNetCore.Identity;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = default!;
    public int? EmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    public Employee? Employee { get; set; }
}