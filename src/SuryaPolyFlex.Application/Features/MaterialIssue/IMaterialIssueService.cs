namespace SuryaPolyFlex.Application.Features.MaterialIssue;

public interface IMaterialIssueService
{
    Task<List<MaterialIssueDto>> GetAllAsync();
    Task<MaterialIssueDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int Id)> CreateAsync(
        CreateMaterialIssueDto dto, string createdBy);
}