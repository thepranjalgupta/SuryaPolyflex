using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class Customer : BaseEntity
{
    public string CustomerCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? BillingAddress { get; set; }
    public string? ShippingAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? GSTIN { get; set; }
    public string? PAN { get; set; }
    public decimal CreditLimit { get; set; } = 0;
    public int PaymentTermDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;
}