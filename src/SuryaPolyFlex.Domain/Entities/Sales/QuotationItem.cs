using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class QuotationItem : BaseEntity
{
    public int QuotationId { get; set; }
    public int? ItemId { get; set; }
    public string Description { get; set; } = default!;
    public decimal Qty { get; set; }
    public int? UoMId { get; set; }
    public decimal UnitRate { get; set; }
    public decimal DiscountPct { get; set; } = 0;
    public decimal TaxPct { get; set; } = 0;
    public decimal LineTotal => Qty * UnitRate * (1 - DiscountPct / 100);

    public Quotation Quotation { get; set; } = default!;
    public Item? Item { get; set; }
    public UnitOfMeasure? UoM { get; set; }
}