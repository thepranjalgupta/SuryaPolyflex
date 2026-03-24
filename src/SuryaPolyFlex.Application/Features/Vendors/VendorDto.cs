using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Vendors;

public class VendorDto
{
    public int Id { get; set; }
    public string VendorCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? GSTIN { get; set; }
    public int PaymentTermDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVendorDto
{
    [Required] public string VendorCode { get; set; } = default!;
    [Required] public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    [EmailAddress] public string? Email { get; set; }
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
}

public class EditVendorDto : CreateVendorDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}