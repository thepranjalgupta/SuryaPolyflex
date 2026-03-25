namespace SuryaPolyFlex.Application.Features.PurchaseOrders;

public interface IPurchaseOrderService
{
    Task<List<PurchaseOrderDto>> GetAllAsync(string? status = null);
    Task<PurchaseOrderDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int POId)> CreateAsync(
        CreatePODto dto, string userName);
    Task<(bool Success, string Message)> ApprovePOAsync(
        int id, string userId, string userName);
    Task<(bool Success, string Message)> CancelPOAsync(
        int id, string userName);
    Task<List<PurchaseOrderDto>> GetApprovedPendingReceiptAsync();
}