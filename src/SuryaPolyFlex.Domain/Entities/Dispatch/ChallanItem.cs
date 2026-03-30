using SuryaPolyFlex.Domain.Entities.Inventory;
using SuryaPolyFlex.Domain.Entities.Sales;

namespace SuryaPolyFlex.Domain.Entities.Dispatch;

public class ChallanItem
{
    public int Id { get; set; }
    public int ChallanId { get; set; }
    public int SOItemId { get; set; }
    public int ItemId { get; set; }
    public decimal DispatchedQty { get; set; }
    public int? UoMId { get; set; }
    public decimal UnitRate { get; set; }

    public DeliveryChallan Challan { get; set; } = default!;
    public SOItem SOItem { get; set; } = default!;
    public Item? Item { get; set; }
}