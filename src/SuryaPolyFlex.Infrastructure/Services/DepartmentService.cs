using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Departments;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;

    public DepartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentDto>> GetAllAsync(string? search = null)
    {
        var query = _context.Departments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d =>
                d.Name.Contains(search) || d.Code.Contains(search));

        return await query
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id          = d.Id,
                Code        = d.Code,
                Name        = d.Name,
                Description = d.Description,
                IsActive    = d.IsActive,
                CreatedAt   = d.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .Where(d => d.Id == id)
            .Select(d => new DepartmentDto
            {
                Id          = d.Id,
                Code        = d.Code,
                Name        = d.Name,
                Description = d.Description,
                IsActive    = d.IsActive,
                CreatedAt   = d.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateDepartmentDto dto, string createdBy)
    {
        var codeExists = await _context.Departments
            .AnyAsync(d => d.Code == dto.Code.ToUpper());

        if (codeExists)
            return (false, $"Department code '{dto.Code}' already exists.");

        _context.Departments.Add(new Department
        {
            Code        = dto.Code.ToUpper().Trim(),
            Name        = dto.Name.Trim(),
            Description = dto.Description,
            IsActive    = true,
            CreatedBy   = createdBy,
            CreatedAt   = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Department created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        EditDepartmentDto dto, string updatedBy)
    {
        var dept = await _context.Departments.FindAsync(dto.Id);
        if (dept == null)
            return (false, "Department not found.");

        var codeExists = await _context.Departments
            .AnyAsync(d => d.Code == dto.Code.ToUpper() && d.Id != dto.Id);

        if (codeExists)
            return (false, $"Department code '{dto.Code}' already exists.");

        dept.Code        = dto.Code.ToUpper().Trim();
        dept.Name        = dto.Name.Trim();
        dept.Description = dto.Description;
        dept.IsActive    = dto.IsActive;
        dept.UpdatedBy   = updatedBy;
        dept.UpdatedAt   = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, "Department updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept == null)
            return (false, "Department not found.");

        var hasEmployees = await _context.Employees
            .AnyAsync(e => e.DepartmentId == id);

        if (hasEmployees)
            return (false, "Cannot delete department with existing employees.");

        dept.IsDeleted = true;
        dept.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Department deleted.");
    }
}