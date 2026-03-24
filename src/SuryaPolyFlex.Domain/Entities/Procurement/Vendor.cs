using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Procurement;

public class Vendor : BaseEntity
{
    public string VendorCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? GSTIN { get; set; }
    public string? PAN { get; set; }
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? BankIFSC { get; set; }
    public int PaymentTermDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;
}