namespace SuryaPolyFlex.Application.Features.GRN;

public interface IGRNService
{
    Task<List<GRNDto>> GetAllAsync();
    Task<GRNDto?> GetByIdAsync(int id);
    Task<List<GRNItemDto>> GetItemsForPOAsync(int poId);
    Task<(bool Success, string Message, int GRNId)> CreateAsync(
        CreateGRNDto dto, string userName);
    Task<(bool Success, string Message)> AcceptAsync(
        int id, string userName);
}