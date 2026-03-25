using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.GRN;

public class GRNDto
{
    public int Id { get; set; }
    public string GRNNumber { get; set; } = default!;
    public DateTime GRNDate { get; set; }
    public int PurchaseOrderId { get; set; }
    public string PONumber { get; set; } = default!;
    public string VendorName { get; set; } = default!;
    public string? VehicleNumber { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public string Status { get; set; } = default!;
    public string WarehouseName { get; set; } = default!;
    public string? Remarks { get; set; }
    public List<GRNItemDto> Items { get; set; } = new();
}

public class GRNItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public string UoMCode { get; set; } = default!;
    public decimal ReceivedQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public string? RejectionReason { get; set; }
    public decimal UnitCost { get; set; }
}

public class CreateGRNDto
{
    [Required] public int PurchaseOrderId { get; set; }
    [Required] public int WarehouseId { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public string? Remarks { get; set; }

    [Required] public List<CreateGRNItemDto> Items { get; set; } = new();
}

public class CreateGRNItemDto
{
    [Required] public int POItemId { get; set; }
    [Required] public int ItemId { get; set; }
    [Required, Range(0.001, double.MaxValue)] public decimal ReceivedQty { get; set; }
    [Required, Range(0, double.MaxValue)]     public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public string? RejectionReason { get; set; }
    public decimal UnitCost { get; set; }
}