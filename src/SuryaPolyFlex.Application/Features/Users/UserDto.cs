using System.ComponentModel.DataAnnotations;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Application.Features.Users;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
    public AuthorityRole AuthorityRole { get; set; } = AuthorityRole.Employee;
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
}

public class CreateUserDto
{
    [Required]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    public string? Phone { get; set; }

    [Required, MinLength(8)]
    public string Password { get; set; } = default!;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = default!;

    [Required]
    public string RoleName { get; set; } = default!;

    public int? EmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public AuthorityRole AuthorityRole { get; set; } = AuthorityRole.Employee;
}

public class EditUserDto
{
    public string Id { get; set; } = default!;

    [Required]
    public string FullName { get; set; } = default!;

    public string? Phone { get; set; }

    [Required]
    public string RoleName { get; set; } = default!;

    public int? EmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public AuthorityRole AuthorityRole { get; set; } = AuthorityRole.Employee;
}

public class ResetPasswordDto
{
    public string UserId { get; set; } = default!;

    [Required, MinLength(8)]
    public string NewPassword { get; set; } = default!;

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = default!;
}