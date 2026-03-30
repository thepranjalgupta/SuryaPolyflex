namespace SuryaPolyFlex.Application.Features.Dispatch;

public interface IDispatchService
{
    // Transporters
    Task<List<TransporterDto>> GetTransportersAsync();
    Task<(bool Success, string Message)> CreateTransporterAsync(
        CreateTransporterDto dto, string createdBy);

    // Challans
    Task<List<DeliveryChallanDto>> GetAllChallansAsync();
    Task<DeliveryChallanDto?> GetChallanByIdAsync(int id);
    Task<List<ChallanItemDto>> GetSOItemsForChallanAsync(int soId);
    Task<(bool Success, string Message, int Id)> CreateChallanAsync(
        CreateChallanDto dto, string createdBy);

    // Shipment
    Task<(bool Success, string Message)> UpdateShipmentStatusAsync(
        UpdateShipmentDto dto, string updatedBy);
}