using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Employees;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeDto>> GetAllAsync(string? search = null)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e =>
                e.FullName.Contains(search) ||
                e.EmployeeCode.Contains(search) ||
                (e.Email != null && e.Email.Contains(search)));

        return await query
            .OrderBy(e => e.FullName)
            .Select(e => new EmployeeDto
            {
                Id             = e.Id,
                EmployeeCode   = e.EmployeeCode,
                FullName       = e.FullName,
                Email          = e.Email,
                Phone          = e.Phone,
                Designation    = e.Designation,
                DepartmentId   = e.DepartmentId,
                DepartmentName = e.Department.Name,
                IsActive       = e.IsActive,
                CreatedAt      = e.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto
            {
                Id             = e.Id,
                EmployeeCode   = e.EmployeeCode,
                FullName       = e.FullName,
                Email          = e.Email,
                Phone          = e.Phone,
                Designation    = e.Designation,
                DepartmentId   = e.DepartmentId,
                DepartmentName = e.Department.Name,
                IsActive       = e.IsActive,
                CreatedAt      = e.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateEmployeeDto dto, string createdBy)
    {
        var codeExists = await _context.Employees
            .AnyAsync(e => e.EmployeeCode == dto.EmployeeCode.ToUpper());

        if (codeExists)
            return (false, $"Employee code '{dto.EmployeeCode}' already exists.");

        _context.Employees.Add(new Employee
        {
            EmployeeCode = dto.EmployeeCode.ToUpper().Trim(),
            FullName     = dto.FullName.Trim(),
            Email        = dto.Email?.Trim(),
            Phone        = dto.Phone?.Trim(),
            Designation  = dto.Designation?.Trim(),
            DepartmentId = dto.DepartmentId,
            IsActive     = true,
            CreatedBy    = createdBy,
            CreatedAt    = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Employee created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        EditEmployeeDto dto, string updatedBy)
    {
        var emp = await _context.Employees.FindAsync(dto.Id);
        if (emp == null)
            return (false, "Employee not found.");

        var codeExists = await _context.Employees
            .AnyAsync(e => e.EmployeeCode == dto.EmployeeCode.ToUpper() && e.Id != dto.Id);

        if (codeExists)
            return (false, $"Employee code '{dto.EmployeeCode}' already exists.");

        emp.EmployeeCode = dto.EmployeeCode.ToUpper().Trim();
        emp.FullName     = dto.FullName.Trim();
        emp.Email        = dto.Email?.Trim();
        emp.Phone        = dto.Phone?.Trim();
        emp.Designation  = dto.Designation?.Trim();
        emp.DepartmentId = dto.DepartmentId;
        emp.IsActive     = dto.IsActive;
        emp.UpdatedBy    = updatedBy;
        emp.UpdatedAt    = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Employee updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null)
            return (false, "Employee not found.");

        emp.IsDeleted = true;
        emp.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Employee deleted.");
    }
}