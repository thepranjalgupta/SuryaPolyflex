using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class SOItem : BaseEntity
{
    public int SalesOrderId { get; set; }
    public int? ItemId { get; set; }
    public string Description { get; set; } = default!;
    public decimal OrderedQty { get; set; }
    public decimal DispatchedQty { get; set; } = 0;
    public decimal PendingQty => OrderedQty - DispatchedQty;
    public int? UoMId { get; set; }
    public decimal UnitRate { get; set; }
    public decimal TaxPct { get; set; } = 0;
    public decimal LineTotal => OrderedQty * UnitRate;

    public SalesOrder SalesOrder { get; set; } = default!;
    public Item? Item { get; set; }
    public UnitOfMeasure? UoM { get; set; }
}