using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class SalesReturnItem : BaseEntity
{
    public int SalesReturnId { get; set; }
    public int? ItemId { get; set; }
    public string Description { get; set; } = default!;
    public decimal ReturnQty { get; set; }
    public int? UoMId { get; set; }
    public decimal UnitRate { get; set; }
    public string? ReturnReason { get; set; }

    public SalesReturn SalesReturn { get; set; } = default!;
    public Item? Item { get; set; }
}