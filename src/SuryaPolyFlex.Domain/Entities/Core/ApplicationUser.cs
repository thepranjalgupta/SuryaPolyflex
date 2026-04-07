using Microsoft.AspNetCore.Identity;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = default!;
    public int? EmployeeId { get; set; }
    public int? DepartmentId { get; set; }

    /// <summary>
    /// User's authority level in the organization (Employee, Manager, DepartmentHead, Admin).
    /// Determines access hierarchy and approval capabilities.
    /// </summary>
    public AuthorityRole AuthorityRole { get; set; } = AuthorityRole.Employee;

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    public Employee? Employee { get; set; }
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}