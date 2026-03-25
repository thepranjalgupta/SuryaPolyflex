using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class GRN : BaseEntity
{
    public string GRNNumber { get; set; } = default!;
    public DateTime GRNDate { get; set; } = DateTime.UtcNow;
    public int PurchaseOrderId { get; set; }
    public int VendorId { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public GrnStatus Status { get; set; } = GrnStatus.Pending;
    public string? QCRemarks { get; set; }
    public string? Remarks { get; set; }
    public int WarehouseId { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Warehouse? Warehouse { get; set; }
    public ICollection<GRNItem> Items { get; set; } = new List<GRNItem>();
}