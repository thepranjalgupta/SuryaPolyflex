using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Domain.Entities.Core;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class Indent : BaseEntity
{
    public string IndentNumber { get; set; } = default!;
    public DateTime IndentDate { get; set; } = DateTime.UtcNow;
    public int DepartmentId { get; set; }
    public string RequestedById { get; set; } = default!;
    public IndentStatus Status { get; set; } = IndentStatus.Draft;
    public string? Remarks { get; set; }
    public string? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Department? Department { get; set; }
    public ICollection<IndentItem> Items { get; set; } = new List<IndentItem>();
}