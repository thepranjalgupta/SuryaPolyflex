using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Inventory;

public class UnitOfMeasure : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}