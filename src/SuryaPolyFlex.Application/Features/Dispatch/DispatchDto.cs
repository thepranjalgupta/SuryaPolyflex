using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Dispatch;

public class TransporterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? GSTIN { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTransporterDto
{
    [Required] public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? GSTIN { get; set; }
}

public class DeliveryChallanDto
{
    public int Id { get; set; }
    public string ChallanNo { get; set; } = default!;
    public DateTime ChallanDate { get; set; }
    public string SONumber { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string? TransporterName { get; set; }
    public string? VehicleNumber { get; set; }
    public string? LRNumber { get; set; }
    public string? EWayBillNo { get; set; }
    public string? DeliveryAddress { get; set; }
    public decimal TotalValue { get; set; }
    public string Status { get; set; } = default!;
    public string? ShipmentStatus { get; set; }
    public List<ChallanItemDto> Items { get; set; } = new();
}

public class ChallanItemDto
{
    public int SOItemId { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public string? UoMCode { get; set; }
    public int? UoMId { get; set; }
    public decimal DispatchedQty { get; set; }
    public decimal UnitRate { get; set; }
    public decimal LineTotal => DispatchedQty * UnitRate;
}

public class CreateChallanDto
{
    [Required] public int SOId { get; set; }
    public int? TransporterId { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? LRNumber { get; set; }
    public string? EWayBillNo { get; set; }
    public string? DeliveryAddress { get; set; }

    public List<CreateChallanItemDto> Items { get; set; } = new();
}

public class CreateChallanItemDto
{
    public int SOItemId { get; set; }
    public int ItemId { get; set; }          // 0 if description-only item
    public string ItemName { get; set; } = default!;
    public string? UoMCode { get; set; }
    public int? UoMId { get; set; }
    [Range(0.001, double.MaxValue)] public decimal DispatchedQty { get; set; }
    public decimal UnitRate { get; set; }
}

public class UpdateShipmentDto
{
    public int ChallanId { get; set; }
    [Required] public string Status { get; set; } = default!;
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? ReceiverName { get; set; }
    public string? DeliveryRemarks { get; set; }
}