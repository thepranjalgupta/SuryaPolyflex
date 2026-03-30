using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class MaterialIssue : BaseEntity
{
    public string IssueNo { get; set; } = default!;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public int FromWarehouseId { get; set; }
    public int? ToDepartmentId { get; set; }
    public int? WorkOrderId { get; set; }
    public string IssuedById { get; set; } = default!;
    public string? Remarks { get; set; }

    public ICollection<MaterialIssueItem> Items { get; set; } = new List<MaterialIssueItem>();
}