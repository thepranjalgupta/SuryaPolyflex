using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public enum ApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public class ApprovalTransaction : BaseEntity
{
    public string Module { get; set; } = default!;
    public int RecordId { get; set; }
    public int CurrentStep { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string CreatedBy { get; set; } = default!;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedStep { get; set; }
    public string? RejectionReason { get; set; }
}