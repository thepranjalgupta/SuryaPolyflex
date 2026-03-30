namespace SuryaPolyFlex.Application.Features.SalesOrders;

public interface ISalesOrderService
{
    Task<List<SalesOrderDto>> GetAllAsync(string? status = null);
    Task<SalesOrderDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int SOId)> CreateAsync(
        CreateSODto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateStatusAsync(
        int id, string status, string updatedBy);
    Task<(bool Success, string Message)> AddCustomerJobAsync(
        CreateCustomerJobDto dto, string createdBy);
}