namespace SuryaPolyFlex.Application.Features.Production;

public interface IProductionService
{
    // Machines
    Task<List<MachineDto>> GetMachinesAsync();
    Task<(bool Success, string Message)> CreateMachineAsync(CreateMachineDto dto, string createdBy);

    // Job Cards
    Task<List<JobCardDto>> GetJobCardsAsync(string? status = null);
    Task<JobCardDto?> GetJobCardByIdAsync(int id);
    Task<(bool Success, string Message, int Id)> CreateJobCardAsync(CreateJobCardDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateJobCardStatusAsync(int id, string status, string updatedBy);

    // BOM
    Task<(bool Success, string Message, int Id)> CreateBOMAsync(CreateBOMDto dto, string createdBy);

    // Work Orders
    Task<List<WorkOrderDto>> GetWorkOrdersAsync(string? status = null);
    Task<WorkOrderDto?> GetWorkOrderByIdAsync(int id);
    Task<(bool Success, string Message, int Id)> CreateWorkOrderAsync(CreateWorkOrderDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateWorkOrderStatusAsync(int id, string status, string updatedBy);

    // Production Entries
    Task<(bool Success, string Message)> AddProductionEntryAsync(CreateProductionEntryDto dto, string createdBy);
    Task<(bool Success, string Message)> CompleteWorkOrderAsync(int workOrderId, int fgWarehouseId, string createdBy);
}