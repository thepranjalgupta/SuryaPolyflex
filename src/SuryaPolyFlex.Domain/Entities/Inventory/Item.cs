using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Inventory;

public class Item : BaseEntity
{
    public string ItemCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int UoMId { get; set; }
    public string ItemType { get; set; } = "RM"; // RM, FG, WIP, Consumable
    public decimal MinStockLevel { get; set; } = 0;
    public decimal ReorderQty { get; set; } = 0;
    public decimal StandardCost { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public ItemCategory Category { get; set; } = default!;
    public UnitOfMeasure UoM { get; set; } = default!;
}