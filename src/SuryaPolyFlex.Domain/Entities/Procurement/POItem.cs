using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class POItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public int ItemId { get; set; }
    public decimal OrderedQty { get; set; }
    public decimal ReceivedQty { get; set; } = 0;
    public decimal PendingQty => OrderedQty - ReceivedQty;
    public decimal UnitPrice { get; set; }
    public decimal TaxPct { get; set; } = 0;
    public decimal LineTotal => OrderedQty * UnitPrice;
    public string? Remarks { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Item? Item { get; set; }
}