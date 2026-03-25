namespace SuryaPolyFlex.Application.Features.Indents;

public interface IIndentService
{
    Task<List<IndentDto>> GetAllAsync(string? status = null);
    Task<IndentDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, int IndentId)> CreateAsync(
        CreateIndentDto dto, string userId, string userName);
    Task<(bool Success, string Message)> SubmitForApprovalAsync(int id, string userId);
    Task<(bool Success, string Message)> ApproveOrRejectAsync(
        ApproveIndentDto dto, string userId, string userName);
}