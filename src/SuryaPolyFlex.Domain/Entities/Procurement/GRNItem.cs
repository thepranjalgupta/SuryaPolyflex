using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class GRNItem : BaseEntity
{
    public int GRNId { get; set; }
    public int POItemId { get; set; }
    public int ItemId { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public string? RejectionReason { get; set; }
    public decimal UnitCost { get; set; }

    public GRN GRN { get; set; } = default!;
    public Item? Item { get; set; }
}