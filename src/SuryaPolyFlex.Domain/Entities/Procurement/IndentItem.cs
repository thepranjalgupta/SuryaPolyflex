using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class IndentItem : BaseEntity
{
    public int IndentId { get; set; }
    public int ItemId { get; set; }
    public decimal RequestedQty { get; set; }
    public decimal ApprovedQty { get; set; }
    public string? Remarks { get; set; }

    public Indent Indent { get; set; } = default!;
    public Item? Item { get; set; }
}