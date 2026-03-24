using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Designation { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;

    public Department Department { get; set; } = default!;
}