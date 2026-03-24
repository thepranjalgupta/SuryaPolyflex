namespace SuryaPolyFlex.Application.Features.Customers;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync(string? search = null);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CreateCustomerDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditCustomerDto dto, string updatedBy);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}