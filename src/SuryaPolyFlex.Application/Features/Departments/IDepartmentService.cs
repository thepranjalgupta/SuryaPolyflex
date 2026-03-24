namespace SuryaPolyFlex.Application.Features.Departments;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync(string? search = null);
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CreateDepartmentDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditDepartmentDto dto, string updatedBy);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}