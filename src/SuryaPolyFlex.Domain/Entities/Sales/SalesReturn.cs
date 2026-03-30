using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class SalesReturn : BaseEntity
{
    public string ReturnNo { get; set; } = default!;
    public int SOId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = default!;
    public decimal TotalReturnValue { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ApprovedById { get; set; }
    public string? Remarks { get; set; }

    public SalesOrder SalesOrder { get; set; } = default!;
    public Customer Customer { get; set; } = default!;
    public ICollection<SalesReturnItem> Items { get; set; } = new List<SalesReturnItem>();
}