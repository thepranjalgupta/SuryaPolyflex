using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class InkReturn : BaseEntity
{
    public string ReturnNo { get; set; } = default!;
    public int JobCardId { get; set; }
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public int ToWarehouseId { get; set; }
    public string ReturnedById { get; set; } = default!;
    public string? Remarks { get; set; }

    public JobCard JobCard { get; set; } = default!;
    public ICollection<InkReturnItem> Items { get; set; } = new List<InkReturnItem>();
}