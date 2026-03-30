using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Entities.Sales;

namespace SuryaPolyFlex.Domain.Entities.Dispatch;

public class DeliveryChallan : BaseEntity
{
    public string ChallanNo { get; set; } = default!;
    public int? DispatchPlanId { get; set; }
    public int SOId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ChallanDate { get; set; } = DateTime.UtcNow;
    public int? TransporterId { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? LRNumber { get; set; }
    public string? EWayBillNo { get; set; }
    public string? DeliveryAddress { get; set; }
    public decimal TotalValue { get; set; }
    public string Status { get; set; } = "Prepared";

    public SalesOrder SalesOrder { get; set; } = default!;
    public Transporter? Transporter { get; set; }
    public DispatchPlan? DispatchPlan { get; set; }
    public ICollection<ChallanItem> Items { get; set; } = new List<ChallanItem>();
    public Shipment? Shipment { get; set; }
}