namespace SuryaPolyFlex.Domain.Enums;

/// <summary>
/// Represents the authority level/hierarchy for a user.
/// This is separate from ApplicationRole (which can be department-specific or function-specific).
/// </summary>
public enum AuthorityRole
{
    /// <summary>
    /// Basic employee with limited access (view own/department records, can create indents/POs).
    /// </summary>
    Employee = 0,

    /// <summary>
    /// Manager with department-level authority (approves department records, manages team).
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Department head with higher approval authority (multiple department approvals).
    /// </summary>
    DepartmentHead = 2,

    /// <summary>
    /// System administrator with full access and override capabilities.
    /// </summary>
    Admin = 3
}
