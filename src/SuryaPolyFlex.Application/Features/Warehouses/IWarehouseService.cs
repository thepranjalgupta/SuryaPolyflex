namespace SuryaPolyFlex.Application.Features.Warehouses;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetAllAsync();
    Task<WarehouseDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CreateWarehouseDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditWarehouseDto dto, string updatedBy);
}