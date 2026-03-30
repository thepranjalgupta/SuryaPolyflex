using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class MaterialIssueItem : BaseEntity
{
    public int MaterialIssueId { get; set; }
    public int ItemId { get; set; }
    public decimal RequestedQty { get; set; }
    public decimal IssuedQty { get; set; }
    public int? UoMId { get; set; }
    public decimal UnitRate { get; set; } = 0;

    public MaterialIssue MaterialIssue { get; set; } = default!;
    public Item? Item { get; set; }
    public UnitOfMeasure? UoM { get; set; }
}