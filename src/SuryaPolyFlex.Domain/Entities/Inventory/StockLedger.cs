using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Inventory;

public class StockLedger
{
    public long Id { get; set; }
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public string ReferenceType { get; set; } = default!; // GRN, WO, SO, etc
    public int ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public decimal InwardQty { get; set; } = 0;
    public decimal OutwardQty { get; set; } = 0;
    public decimal BalanceQty { get; set; } = 0;
    public decimal UnitCost { get; set; } = 0;
    public string? Remarks { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Item Item { get; set; } = default!;
    public Warehouse Warehouse { get; set; } = default!;
}