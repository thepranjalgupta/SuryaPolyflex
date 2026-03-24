namespace SuryaPolyFlex.Domain.Entities.Inventory;

public class StockBalance
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public decimal OnHandQty { get; set; } = 0;
    public decimal ReservedQty { get; set; } = 0;
    public decimal AvailableQty => OnHandQty - ReservedQty;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    public Item Item { get; set; } = default!;
    public Warehouse Warehouse { get; set; } = default!;
}