namespace SuryaPolyFlex.Application.Features.Employees;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync(string? search = null);
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CreateEmployeeDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditEmployeeDto dto, string updatedBy);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}