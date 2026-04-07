namespace SuryaPolyFlex.Domain.Entities.Core;

/// <summary>
/// Represents a user-level permission override.
/// Allows admins to grant/deny specific permissions to users, overriding role-based access.
/// </summary>
public class UserPermission
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public int PermissionId { get; set; }

    /// <summary>
    /// If true, the user is explicitly allowed this permission (overrides role-based denial).
    /// If false, the user is explicitly denied this permission (overrides role-based allowance).
    /// </summary>
    public bool IsAllowed { get; set; } = true;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional expiry date. If set and in the past, the override is ignored.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ApplicationUser User { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}
