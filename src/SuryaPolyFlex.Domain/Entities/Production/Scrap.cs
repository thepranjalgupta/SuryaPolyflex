using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class Scrap : BaseEntity
{
    public string ScrapNo { get; set; } = default!;
    public int JobCardId { get; set; }
    public DateTime ScrapDate { get; set; } = DateTime.UtcNow;
    public int ItemId { get; set; }
    public decimal ScrapQty { get; set; }
    public int? UoMId { get; set; }
    public string? ScrapType { get; set; }
    public string? DisposalMethod { get; set; }
    public DateTime? DisposalDate { get; set; }
    public decimal DisposalValue { get; set; } = 0;

    public JobCard JobCard { get; set; } = default!;
    public Item? Item { get; set; }
}