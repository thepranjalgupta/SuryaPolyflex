using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.GateEntry;

public class GateEntryDto
{
    public int Id { get; set; }
    public string GateEntryNo { get; set; } = default!;
    public string VendorName { get; set; } = default!;
    public string? PONumber { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public DateTime EntryDateTime { get; set; }
    public DateTime? ExitDateTime { get; set; }
    public string ReceivedBy { get; set; } = default!;
    public string? Remarks { get; set; }
    public bool GRNCreated { get; set; }
}

public class CreateGateEntryDto
{
    [Required] public int VendorId { get; set; }
    public int? POId { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    [Required] public DateTime EntryDateTime { get; set; } = DateTime.Now;
    [Required] public string ReceivedBy { get; set; } = default!;
    public string? Remarks { get; set; }
}