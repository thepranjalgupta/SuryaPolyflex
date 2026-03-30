namespace SuryaPolyFlex.Application.Features.Leads;

public interface ILeadService
{
    Task<List<LeadDto>> GetAllAsync(string? status = null);
    Task<LeadDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message)> CreateAsync(CreateLeadDto dto, string createdBy);
    Task<(bool Success, string Message)> UpdateAsync(EditLeadDto dto, string updatedBy);
    Task<(bool Success, string Message)> ConvertToCustomerAsync(int id, string updatedBy);
}