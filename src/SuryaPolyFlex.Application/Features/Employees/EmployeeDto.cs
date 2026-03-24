using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Employees;

public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Designation { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEmployeeDto
{
    [Required]
    public string EmployeeCode { get; set; } = default!;

    [Required]
    public string FullName { get; set; } = default!;

    [EmailAddress]
    public string? Email { get; set; }

    public string? Phone { get; set; }
    public string? Designation { get; set; }

    [Required]
    public int DepartmentId { get; set; }
}

public class EditEmployeeDto
{
    public int Id { get; set; }

    [Required]
    public string EmployeeCode { get; set; } = default!;

    [Required]
    public string FullName { get; set; } = default!;

    [EmailAddress]
    public string? Email { get; set; }

    public string? Phone { get; set; }
    public string? Designation { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    public bool IsActive { get; set; }
}