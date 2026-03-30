using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class BOMItem : BaseEntity
{
    public int BOMId { get; set; }
    public int ItemId { get; set; }
    public decimal RequiredQty { get; set; }
    public decimal IssuedQty { get; set; } = 0;
    public int? UoMId { get; set; }

    public BOM BOM { get; set; } = default!;
    public Item? Item { get; set; }
    public UnitOfMeasure? UoM { get; set; }
}