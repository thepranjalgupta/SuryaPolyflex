namespace SuryaPolyFlex.Domain.Entities.Core;

public class WorkflowAction
{
    public int Id { get; set; }
    public string ModuleCode { get; set; } = default!;   // INDENT, PO, SO
    public int ReferenceId { get; set; }
    public string Action { get; set; } = default!;        // Submitted, Approved, Rejected
    public string ActionById { get; set; } = default!;
    public string? ActionByName { get; set; }
    public DateTime ActionAt { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
}