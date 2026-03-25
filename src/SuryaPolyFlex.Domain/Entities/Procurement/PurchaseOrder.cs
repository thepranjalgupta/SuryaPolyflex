using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class PurchaseOrder : BaseEntity
{
    public string PONumber { get; set; } = default!;
    public DateTime PODate { get; set; } = DateTime.UtcNow;
    public int VendorId { get; set; }
    public int? IndentId { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Open;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Terms { get; set; }
    public string? Remarks { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Vendor Vendor { get; set; } = default!;
    public Indent? Indent { get; set; }
    public ICollection<POItem> Items { get; set; } = new List<POItem>();
}