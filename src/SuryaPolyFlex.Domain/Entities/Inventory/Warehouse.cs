using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Inventory;

public class Warehouse : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public string WarehouseType { get; set; } = "RM"; // RM, FG, WIP, Scrap
    public bool IsActive { get; set; } = true;
}