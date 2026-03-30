using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class BOM : BaseEntity
{
    public string BOMNo { get; set; } = default!;
    public int JobCardId { get; set; }
    public string RequestedById { get; set; } = default!;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public string? Remarks { get; set; }

    public JobCard JobCard { get; set; } = default!;
    public ICollection<BOMItem> Items { get; set; } = new List<BOMItem>();
}