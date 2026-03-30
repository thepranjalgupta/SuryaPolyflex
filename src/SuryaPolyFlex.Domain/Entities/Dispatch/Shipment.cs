using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Dispatch;

public class Shipment
{
    public int Id { get; set; }
    public int ChallanId { get; set; }
    public ShipmentStatus CurrentStatus { get; set; } = ShipmentStatus.Dispatched;
    public DateTime StatusUpdatedAt { get; set; } = DateTime.UtcNow;
    public string StatusUpdatedBy { get; set; } = default!;
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? PODDocument { get; set; }
    public string? ReceiverName { get; set; }
    public string? DeliveryRemarks { get; set; }

    public DeliveryChallan Challan { get; set; } = default!;
}