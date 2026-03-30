using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class GateEntry : BaseEntity
{
    public string GateEntryNo { get; set; } = default!;
    public int? POId { get; set; }
    public int VendorId { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public DateTime EntryDateTime { get; set; } = DateTime.UtcNow;
    public DateTime? ExitDateTime { get; set; }
    public string ReceivedBy { get; set; } = default!;
    public string? Remarks { get; set; }
    public int? GRNId { get; set; }

    public Vendor Vendor { get; set; } = default!;
    public PurchaseOrder? PurchaseOrder { get; set; }
}