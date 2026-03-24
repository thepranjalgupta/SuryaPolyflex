using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Inventory;

public class ItemCategory : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public ICollection<Item> Items { get; set; } = new List<Item>();
}