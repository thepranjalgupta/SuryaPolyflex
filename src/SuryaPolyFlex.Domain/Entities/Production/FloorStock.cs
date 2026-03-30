using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class FloorStock
{
    public int Id { get; set; }
    public int JobCardId { get; set; }
    public int ItemId { get; set; }
    public decimal IssuedQty { get; set; } = 0;
    public decimal ConsumedQty { get; set; } = 0;
    public decimal ReturnedQty { get; set; } = 0;
    public decimal BalanceQty => IssuedQty - ConsumedQty - ReturnedQty;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    public JobCard JobCard { get; set; } = default!;
    public Item? Item { get; set; }
}