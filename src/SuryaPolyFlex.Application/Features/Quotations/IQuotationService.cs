namespace SuryaPolyFlex.Application.Features.Quotations;

public interface IQuotationService
{
    Task<List<QuotationDto>> GetAllAsync(string? status = null);
    Task<QuotationDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int QuotationId)> CreateAsync(
        CreateQuotationDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateStatusAsync(
        int id, string status, string updatedBy);
}