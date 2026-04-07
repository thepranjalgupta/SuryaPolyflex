using SuryaPolyFlex.Domain.Entities.Core;

namespace SuryaPolyFlex.Application.Common.Interfaces;

public interface IApprovalService
{
    Task<ApprovalTransaction> CreateApprovalTransactionAsync(string module, int recordId, string creatorId, int? creatorDepartmentId);
    Task<ApprovalTransaction?> GetApprovalTransactionAsync(string module, int recordId);
    Task<bool> CanApproveAsync(string userId, string module, int currentStep, int? approverDepartmentId);
    Task<ApprovalTransaction> ApproveAsync(string userId, string module, int recordId, int currentStep);
    Task<ApprovalTransaction> RejectAsync(string userId, string module, int recordId, string reason);
    Task<List<ApprovalTransaction>> GetPendingApprovalsAsync(string userId);
}