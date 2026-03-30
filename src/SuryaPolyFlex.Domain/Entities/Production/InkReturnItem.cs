using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class InkReturnItem : BaseEntity
{
    public int InkReturnId { get; set; }
    public int ItemId { get; set; }
    public decimal ReturnQty { get; set; }
    public int? UoMId { get; set; }

    public InkReturn InkReturn { get; set; } = default!;
    public Item? Item { get; set; }
}